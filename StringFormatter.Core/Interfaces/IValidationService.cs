using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringFormatter.Core.Dtos;
using StringFormatter.Core.Models;

namespace StringFormatter.Core.Interfaces
{
    public interface IValidationService
    {
        ValidationResult ValidateInterpolationUnit(InterpolationUnitDto unit);
        ValidationResult ValidateEscapeCharacter(EscapeCharacter escapeCharacter);
    }
}