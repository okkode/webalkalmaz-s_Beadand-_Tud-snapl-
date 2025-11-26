using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GdeWebModels
{
    public class NoteModel
    {
        public int NoteId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime NoteDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }

    public class NoteListModel
    {
        public List<NoteModel> Notes { get; set; } = new();
    }

    public class MonthlySummaryModel
    {
        public int SummaryId { get; set; }
        public int UserId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string SummaryText { get; set; } = "";
        public string LearningReflection { get; set; } = "";
        public DateTime ModificationDate { get; set; }
    }

    public class AudioNoteModel
    {
        public int AudioNoteId { get; set; }
        public int UserId { get; set; }
        public string FilePath { get; set; } = "";
        public string? Transcript { get; set; }
        public DateTime NoteDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}
