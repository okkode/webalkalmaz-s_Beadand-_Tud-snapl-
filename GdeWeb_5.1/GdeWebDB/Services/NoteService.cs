using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GdeWebDB.Entities;
using GdeWebDB.Interfaces;
using GdeWebModels;
using Microsoft.EntityFrameworkCore;



namespace GdeWebDB.Services
{
    public class NoteService : INoteService
    {
        private readonly GdeDbContext _ctx;
        private readonly IAiClient _aiClient;


        public NoteService(GdeDbContext ctx, IAiClient aiClient)
        {
            _ctx = ctx;
            _aiClient = aiClient;
        }


        public async Task<NoteListModel> GetNotesForUser(int userId)
        {
            var list = await _ctx.Notes
                .Where(n => n.USERID == userId)
                .OrderByDescending(n => n.NOTEDATE)
                .ToListAsync();

            return new NoteListModel
            {
                Notes = list.Select(n => new NoteModel
                {
                    NoteId = n.NOTEID,
                    UserId = n.USERID,
                    Title = n.NOTETITLE,
                    Content = n.NOTECONTENT,
                    NoteDate = n.NOTEDATE,
                    ModificationDate = n.MODIFICATIONDATE
                }).ToList()
            };
        }

        public async Task<NoteModel> GetNoteById(int userId, int noteId)
        {
            var n = await _ctx.Notes
                .FirstOrDefaultAsync(x => x.NOTEID == noteId && x.USERID == userId);

            if (n == null) throw new Exception("Not found or unauthorized.");

            return new NoteModel
            {
                NoteId = n.NOTEID,
                UserId = n.USERID,
                Title = n.NOTETITLE,
                Content = n.NOTECONTENT,
                NoteDate = n.NOTEDATE,
                ModificationDate = n.MODIFICATIONDATE
            };
        }

        public async Task<ResultModel> AddOrUpdateNote(int userId, NoteModel model)
        {
            Note entity;

            if (model.NoteId == 0)
            {
                entity = new Note
                {
                    USERID = userId,
                    NOTETITLE = model.Title,
                    NOTECONTENT = model.Content,
                    NOTEDATE = model.NoteDate,
                    MODIFICATIONDATE = DateTime.UtcNow
                };

                _ctx.Notes.Add(entity);
            }
            else
            {
                entity = await _ctx.Notes.FirstOrDefaultAsync(x =>
                    x.NOTEID == model.NoteId && x.USERID == userId);

                if (entity == null)
                    throw new Exception("Not found or unauthorized.");

                entity.NOTETITLE = model.Title;
                entity.NOTECONTENT = model.Content;
                entity.NOTEDATE = model.NoteDate;
                entity.MODIFICATIONDATE = DateTime.UtcNow;
            }

            await _ctx.SaveChangesAsync();
            return new ResultModel { Success = true };
        }

        public async Task<ResultModel> DeleteNote(int userId, int noteId)
        {
            var entity = await _ctx.Notes.FirstOrDefaultAsync(x =>
                x.NOTEID == noteId && x.USERID == userId);

            if (entity == null)
                throw new Exception("Not found or unauthorized.");

            _ctx.Notes.Remove(entity);
            await _ctx.SaveChangesAsync();

            return new ResultModel { Success = true };
        }

        public async Task<MonthlySummaryModel> GetMonthlySummary(int userId, int year, int month)
        {
            var m = await _ctx.MonthlySummaries
                .FirstOrDefaultAsync(x =>
                    x.USERID == userId &&
                    x.YEAR == year &&
                    x.MONTH == month);

            if (m == null) return new MonthlySummaryModel
            {
                UserId = userId,
                Year = year,
                Month = month,
                SummaryText = "",
                LearningReflection = ""
            };

            return new MonthlySummaryModel
            {
                SummaryId = m.SUMMARYID,
                UserId = m.USERID,
                Year = m.YEAR,
                Month = m.MONTH,
                SummaryText = m.SUMMARYTEXT,
                LearningReflection = m.LEARNINGREFLECTION,
                ModificationDate = m.MODIFICATIONDATE
            };
        }

        public async Task<ResultModel> SaveMonthlySummary(int userId, MonthlySummaryModel model)
        {
            MonthlySummary entity;

            if (model.SummaryId == 0)
            {
                entity = new MonthlySummary
                {
                    USERID = userId,
                    YEAR = model.Year,
                    MONTH = model.Month,
                    SUMMARYTEXT = model.SummaryText,
                    LEARNINGREFLECTION = model.LearningReflection,
                    MODIFICATIONDATE = DateTime.UtcNow
                };
                _ctx.MonthlySummaries.Add(entity);
            }
            else
            {
                entity = await _ctx.MonthlySummaries.FirstOrDefaultAsync(x =>
                    x.SUMMARYID == model.SummaryId &&
                    x.USERID == userId);

                if (entity == null) throw new Exception("Not found or unauthorized.");

                entity.SUMMARYTEXT = model.SummaryText;
                entity.LEARNINGREFLECTION = model.LearningReflection;
                entity.MODIFICATIONDATE = DateTime.UtcNow;
            }

            await _ctx.SaveChangesAsync();
            return new ResultModel { Success = true };
        }

        public async Task<ResultModel> AddAudioNote(int userId, AudioNoteModel model)
        {
            var entity = new AudioNote
            {
                USERID = userId,
                FILEPATH = model.FilePath,
                TRANSCRIPT = model.Transcript,
                NOTEDATE = model.NoteDate,
                MODIFICATIONDATE = DateTime.UtcNow
            };

            _ctx.AudioNotes.Add(entity);
            await _ctx.SaveChangesAsync();

            return new ResultModel { Success = true };
        }

        public async Task<List<AudioNoteModel>> GetAudioNotesForUser(int userId)
        {
            var list = await _ctx.AudioNotes
                .Where(x => x.USERID == userId)
                .OrderByDescending(x => x.NOTEDATE)
                .ToListAsync();

            return list.Select(a => new AudioNoteModel
            {
                AudioNoteId = a.AUDIONOTEID,
                UserId = a.USERID,
                FilePath = a.FILEPATH,
                Transcript = a.TRANSCRIPT,
                NoteDate = a.NOTEDATE,
                ModificationDate = a.MODIFICATIONDATE
            }).ToList();
        }
    
    public async Task<ResultModel> GenerateMonthlySummary(int userId, int year, int month)
        {
            // 1) Összegyűjtjük a felhasználó adott hónapra eső jegyzeteit
            var notes = await _ctx.Notes
                .Where(n => n.USERID == userId &&
                            n.NOTEDATE.Year == year &&
                            n.NOTEDATE.Month == month)
                .OrderBy(n => n.NOTEDATE)
                .ToListAsync();

            if (!notes.Any())
            {
                return new ResultModel
                {
                    Success = false,
                    ErrorMessage = "Nincs jegyzet ebben a hónapban."
                };
            }

            // 2) Prompt építése az AI-nak
            var promptBuilder = new System.Text.StringBuilder();
            promptBuilder.AppendLine($"Készíts részletes, magyar nyelvű összefoglalót arról, hogy mit tanult a felhasználó {year}.{month:00}-ben.");
            promptBuilder.AppendLine("Az alábbi jegyzetek alapján foglald össze a fő témákat, fontos fogalmakat, és adj rövid reflexiót is:");
            promptBuilder.AppendLine();

            foreach (var n in notes)
            {
                promptBuilder.AppendLine($"[{n.NOTEDATE:yyyy-MM-dd}] {n.NOTETITLE}");
                promptBuilder.AppendLine(n.NOTECONTENT);
                promptBuilder.AppendLine();
            }

            var prompt = promptBuilder.ToString();

            // 3) AI meghívása (DummyAiClient vagy valódi kliens)
            var summaryText = await _aiClient.SummarizeAsync(prompt);

            // 4) MonthlySummary mentése (insert vagy update)
            var entity = await _ctx.MonthlySummaries.FirstOrDefaultAsync(x =>
                x.USERID == userId &&
                x.YEAR == year &&
                x.MONTH == month);

            if (entity == null)
            {
                entity = new MonthlySummary
                {
                    USERID = userId,
                    YEAR = year,
                    MONTH = month,
                    SUMMARYTEXT = summaryText,
                    LEARNINGREFLECTION = "", // opcionálisan külön mező
                    MODIFICATIONDATE = DateTime.UtcNow
                };
                _ctx.MonthlySummaries.Add(entity);
            }
            else
            {
                entity.SUMMARYTEXT = summaryText;
                entity.MODIFICATIONDATE = DateTime.UtcNow;
            }

            await _ctx.SaveChangesAsync();

            return new ResultModel { Success = true };
        }

    } 
}

