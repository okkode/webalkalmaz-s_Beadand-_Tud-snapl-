using GdeWebAPI.Middleware;
using GdeWebAPI.Utilities;
using GdeWebDB.Interfaces;
using GdeWebModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace GdeWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }

        private int GetUserId()
        {
            var accessToken = Request.Headers["AccessToken"].ToString();

            // ✅ Tesztkörnyezet: ha a token TEST_TOKEN, akkor adjunk vissza egy fix userId-t (pl. 1)
            if (accessToken == "TEST_TOKEN")
            {
                return 1;
            }

            // Élesben a rendes JWT-t dekódoljuk
            return GdeWebAPI.Utilities.Utilities.GetUserIdFromToken(accessToken);
        }



        [HttpPost("GetNotes")]
        [ServiceFilter(typeof(AccessTokenFilter))]
        public async Task<NoteListModel> GetNotes()
        {
            int userId = GetUserId();
            return await _noteService.GetNotesForUser(userId);
        }

        [HttpPost("SaveNote")]
        [ServiceFilter(typeof(AccessTokenFilter))]
        public async Task<ResultModel> SaveNote([FromBody] NoteModel model)
        {
            int userId = GetUserId();
            return await _noteService.AddOrUpdateNote(userId, model);
        }

        [HttpPost("DeleteNote")]
        [ServiceFilter(typeof(AccessTokenFilter))]
        public async Task<ResultModel> DeleteNote([FromBody] int noteId)
        {
            int userId = GetUserId();
            return await _noteService.DeleteNote(userId, noteId);
        }

        [HttpPost("GetMonthlySummary")]
        [ServiceFilter(typeof(AccessTokenFilter))]
        public async Task<MonthlySummaryModel> GetMonthlySummary([FromBody] MonthlySummaryModel model)
        {
            int userId = GetUserId();
            return await _noteService.GetMonthlySummary(userId, model.Year, model.Month);
        }

        [HttpPost("SaveMonthlySummary")]
        [ServiceFilter(typeof(AccessTokenFilter))]
        public async Task<ResultModel> SaveMonthlySummary([FromBody] MonthlySummaryModel model)
        {
            int userId = GetUserId();
            return await _noteService.SaveMonthlySummary(userId, model);
        }

        [HttpPost("AddAudioNote")]
        [ServiceFilter(typeof(AccessTokenFilter))]
        public async Task<ResultModel> AddAudioNote([FromBody] AudioNoteModel model)
        {
            int userId = GetUserId();
            return await _noteService.AddAudioNote(userId, model);
        }

        [HttpPost("GetAudioNotes")]
        [ServiceFilter(typeof(AccessTokenFilter))]
        public async Task<List<AudioNoteModel>> GetAudioNotes()
        {
            int userId = GetUserId();
            return await _noteService.GetAudioNotesForUser(userId);
        }
    }
}

