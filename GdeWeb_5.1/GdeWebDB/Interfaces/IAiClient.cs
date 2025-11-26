using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks;

namespace GdeWebDB.Interfaces
{
    public interface IAiClient
    {
        Task<string> SummarizeAsync(string prompt);
    }
}

