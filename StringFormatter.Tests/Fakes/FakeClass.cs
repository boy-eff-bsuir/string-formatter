using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringFormatter.Tests.Fakes
{
    public class FakeClass
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public string[] Children { get; set; } = { "Suzy", "Emily" };
    }
}