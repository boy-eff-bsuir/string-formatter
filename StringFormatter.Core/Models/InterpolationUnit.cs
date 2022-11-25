using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringFormatter.Core.Models
{
    public record InterpolationUnit(string Name, bool IsArray, int Index = 0);
}