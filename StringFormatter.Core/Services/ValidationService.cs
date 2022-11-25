using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringFormatter.Core.Dtos;
using StringFormatter.Core.Interfaces;
using StringFormatter.Core.Models;

namespace StringFormatter.Core.Services
{
    public class ValidationService : IValidationService
    {
        public ValidationResult ValidateEscapeCharacter(EscapeCharacter escapeCharacter)
        {
            if (!escapeCharacter.IsClosed)
            {
                return new ValidationResult(false, $"Escape character is not closed at {escapeCharacter.Position}");
            }
            return new ValidationResult(true, null);
        }

        public ValidationResult ValidateInterpolationUnit(InterpolationUnitDto unit)
        {
            if (!unit.IsClosed)
            {
                return new ValidationResult(false, $"Interpolation unit is not closed at {unit.OpenCurlyBracketPosition}");
            }

            if (unit.OpenSquareBracketPosition != -1 && unit.CloseSquareBracketPosition == -1)
            {
                return new ValidationResult(false, $"Interpolation unit is not closed at {unit.OpenSquareBracketPosition}");
            }

            return new ValidationResult(true, null);
        }
    }
}