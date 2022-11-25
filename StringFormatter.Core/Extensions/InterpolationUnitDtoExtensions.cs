using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringFormatter.Core.Dtos;
using StringFormatter.Core.Models;

namespace StringFormatter.Core.Extensions
{
    public static class InterpolationUnitDtoExtensions
    {
        public static InterpolationUnit ToUnit(this InterpolationUnitDto dto, string template)
        {
            var isArray = dto.OpenSquareBracketPosition > -1 && dto.CloseSquareBracketPosition > -1;
            int index = 0;
            string name = null;
            if (isArray)
            {
                name = template.Substring(dto.OpenCurlyBracketPosition + 1,
                    dto.OpenSquareBracketPosition - dto.OpenCurlyBracketPosition - 1).Trim();
                index = Int32.Parse(template.Substring(dto.OpenSquareBracketPosition + 1, 
                    dto.CloseSquareBracketPosition - dto.OpenSquareBracketPosition - 1));
            }
            else
            {
                name = template.Substring(dto.OpenCurlyBracketPosition + 1, 
                    dto.CloseCurlyBracketPosition - dto.OpenCurlyBracketPosition - 1).Trim();
            }
            return new InterpolationUnit(name, isArray, index);
        }
    }
}