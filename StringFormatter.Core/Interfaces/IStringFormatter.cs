using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringFormatter.Core.Interfaces
{
    public interface IStringFormatter
    {
        string Format(string template, object target);
    }
}