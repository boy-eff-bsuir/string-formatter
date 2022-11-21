using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace StringFormatter.Core.Exceptions
{
    public class WrongStringException : Exception
    {
        public WrongStringException()
        {
        }

        public WrongStringException(string? message) : base(message)
        {
        }

        public WrongStringException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected WrongStringException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}