using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringFormatter.Core.Models
{
    public struct DictionaryKey
    {
        public Type Type { get; set; }
        public string MemberName { get; set; }
        public int Index { get; set; }
    }
}