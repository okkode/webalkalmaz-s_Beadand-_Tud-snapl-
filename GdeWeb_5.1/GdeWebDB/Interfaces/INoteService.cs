using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GdeWebModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GdeWebDB.Interfaces
{
    public interface INoteService
    {
        Task<NoteListModel> GetNotesForUser(int userId);
        Task<NoteModel> GetNoteById(int userId, int noteId);
        Task<ResultModel> AddOrUpdateNote(int userId, NoteModel model);
        Task<ResultModel> DeleteNote(int userId, int noteId);

        Task<MonthlySummaryModel> GetMonthlySummary(int userId, int year, int month);
        Task<ResultModel> SaveMonthlySummary(int userId, MonthlySummaryModel model);

        Task<ResultModel> AddAudioNote(int userId, AudioNoteModel model);
        Task<List<AudioNoteModel>> GetAudioNotesForUser(int userId);
        Task<ResultModel> GenerateMonthlySummary(int userId, int year, int month);

    }
}

