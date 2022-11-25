using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringFormatter.Core.Models
{
    public class ArrayIndex
    {
        public ArrayIndex(int openPosition)
        {
            OpenPosition = openPosition;
        }

        public int OpenPosition { get; set; }
        public int ClosePosition { get; set; } = -1;
        public bool IsClosed => ClosePosition != -1;
    }
}