using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringFormatter.Core.Exceptions;
using StringFormatter.Core.Interfaces;
using StringFormatter.Core.Models;

namespace StringFormatter.Core
{
    public class Formatter : IStringFormatter
    {
        private readonly Stack<InterpolationUnit> _units = new();
        private readonly Stack<EscapeCharacter> _escapeChars = new();
        private const char OpenCurlyBracket = '{';
        private const char CloseCurlyBracket = '}';

        public string Format(string template, object target)
        {
            for (int i = 0; i < template.Length; i++)
            {
                if (template[i] == OpenCurlyBracket)
                {
                    ProcessOpenCurlyBracket(i);
                }
                else if (template[i] == CloseCurlyBracket)
                {
                    ProcessCloseCurlyBracket(i);
                }
            }

            Validate();
            return String.Empty;
        }

        private void ProcessOpenCurlyBracket(int pos)
        {
            if (_units.Any() && _units.Peek().OpenPosition == pos - 1)
            {
                _escapeChars.Push(new(OpenCurlyBracket, pos - 1, true));
                _units.Pop();
            }
            else
            {
                _units.Push(new InterpolationUnit() { OpenPosition = pos });
            }
        }

        private void ProcessCloseCurlyBracket(int pos)
        {
            if (_units.Any() && !_units.Peek().IsClosed)
            {
                var unit = _units.Peek().ClosePosition = pos;
            }
            else if (_escapeChars.Any())
            {
                var ch = _escapeChars.Peek();
                if (ch.Char == CloseCurlyBracket && !ch.IsClosed)
                {
                    ch.IsClosed = true;
                }
                else
                {
                    _escapeChars.Push(new EscapeCharacter(CloseCurlyBracket, pos, false));
                }
            }
            else
            {
                _escapeChars.Push(new EscapeCharacter(CloseCurlyBracket, pos, false));
            }
        }

        private void Validate()
        {
            foreach (var unit in _units)
            {
                if (!unit.IsClosed)
                {
                    throw new WrongStringException($"Wrong string starting at {unit.OpenPosition}");
                }
            }

            foreach (var ch in _escapeChars)
            {
                if (!ch.IsClosed)
                {
                    throw new WrongStringException($"Wrong string starting at {ch.Position}");
                }
            }
        }
    }

    
}