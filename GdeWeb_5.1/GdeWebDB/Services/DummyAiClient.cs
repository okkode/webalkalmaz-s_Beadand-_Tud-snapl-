using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GdeWebDB.Interfaces;

namespace GdeWebDB.Services
{
    /// <summary>
    /// Egyszerű, „butus” AI kliens, ami mindig valami fix szöveget ad vissza.
    /// Élesben ezt le lehet cserélni valódi OpenAI hívásra.
    /// </summary>
    public class DummyAiClient : IAiClient
    {
        public Task<string> SummarizeAsync(string prompt)
        {
            // Itt később lehetne OpenAI hívás – a beadandóhoz elég egy fix szöveg.
            return Task.FromResult("Összefoglaló (AI dummy): " + prompt.Substring(0, System.Math.Min(50, prompt.Length)) + "...");
        }
    }
}
