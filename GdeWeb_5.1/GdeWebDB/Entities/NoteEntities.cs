using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GdeWebDB.Entities
{
    public class Note
    {
        public int NOTEID { get; set; }
        public int USERID { get; set; }
        public string NOTETITLE { get; set; } = "";
        public string NOTECONTENT { get; set; } = "";
        public DateTime NOTEDATE { get; set; }
        public DateTime MODIFICATIONDATE { get; set; }
        public User User { get; set; } = default!;
    }

    public class MonthlySummary
    {
        public int SUMMARYID { get; set; }
        public int USERID { get; set; }
        public int YEAR { get; set; }
        public int MONTH { get; set; }
        public string SUMMARYTEXT { get; set; } = "";
        public string LEARNINGREFLECTION { get; set; } = "";
        public DateTime MODIFICATIONDATE { get; set; }
        public User User { get; set; } = default!;
    }

    public class AudioNote
    {
        public int AUDIONOTEID { get; set; }
        public int USERID { get; set; }
        public string FILEPATH { get; set; } = "";
        public string? TRANSCRIPT { get; set; }
        public DateTime NOTEDATE { get; set; }
        public DateTime MODIFICATIONDATE { get; set; }
        public User User { get; set; } = default!;
    }
}

