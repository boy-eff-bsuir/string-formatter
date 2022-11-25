using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringFormatter.Core.Models
{
    public record ValidationResult(bool Succeed, string ErrorMessage);
}