using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringFormatter.Core.Dtos
{
    public class InterpolationUnitDto
    {
        public InterpolationUnitDto(int openCurlyBracketPosition)
        {
            OpenCurlyBracketPosition = openCurlyBracketPosition;
        }

        public int OpenCurlyBracketPosition { get; set; }
        public int CloseCurlyBracketPosition { get; set; } = -1;
        public int OpenSquareBracketPosition { get; set; } = -1;
        public int CloseSquareBracketPosition { get; set; } = -1;
        public bool IsClosed => CloseCurlyBracketPosition != -1;
    }
}