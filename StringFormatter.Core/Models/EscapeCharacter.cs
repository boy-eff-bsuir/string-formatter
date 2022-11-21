using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringFormatter.Core.Models
{
    public class EscapeCharacter
    {
        public EscapeCharacter(char ch, int position, bool isClosed)
        {
            Char = ch;
            Position = position;
            IsClosed = isClosed;
        }

        public char Char { get; set; }
        public int Position { get; set; }
        public bool IsClosed { get; set; }
    }
}