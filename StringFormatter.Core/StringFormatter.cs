using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringFormatter.Core.Exceptions;
using StringFormatter.Core.Interfaces;
using StringFormatter.Core.Models;

namespace StringFormatter.Core
{
    public class Formatter : IStringFormatter
    {
        private const char OpenCurlyBracket = '{';
        private const char CloseCurlyBracket = '}';
        private static ConcurrentDictionary<DictionaryKey, Delegate> _cache = new();

        public string Format(string template, object target)
        {
            Stack<InterpolationUnit> units = new();
            Stack<EscapeCharacter> escapeChars = new();
            ProcessInputString(template, units, escapeChars);
            Validate(units, escapeChars);
            
            var result = ReplaceInterpolationUnits(template, target, units, escapeChars);
            return result;
        }

        private void ProcessInputString(string template,
            Stack<InterpolationUnit> units, 
            Stack<EscapeCharacter> escapeChars)
        {
            for (int i = 0; i < template.Length; i++)
            {
                if (template[i] == OpenCurlyBracket)
                {
                    ProcessOpenCurlyBracket(i, units, escapeChars);
                }
                else if (template[i] == CloseCurlyBracket)
                {
                    ProcessCloseCurlyBracket(i, units, escapeChars);
                }
            }
        }

        private void ProcessOpenCurlyBracket(int pos,
            Stack<InterpolationUnit> units, 
            Stack<EscapeCharacter> escapeChars)
        {
            if (units.Any() && units.Peek().OpenPosition == pos - 1)
            {
                escapeChars.Push(new(OpenCurlyBracket, pos - 1, true));
                units.Pop();
            }
            else
            {
                units.Push(new InterpolationUnit() { OpenPosition = pos });
            }
        }

        private void ProcessCloseCurlyBracket(int pos,
            Stack<InterpolationUnit> units, 
            Stack<EscapeCharacter> escapeChars)
        {
            if (units.Any() && !units.Peek().IsClosed)
            {
                var unit = units.Peek().ClosePosition = pos;
                return;
            }

            EscapeCharacter ch;
            var isNotEmpty = escapeChars.TryPeek(out ch);
            if (isNotEmpty && ch.Char == CloseCurlyBracket && !ch.IsClosed)
            {
                ch.IsClosed = true;
                return;
            }

            escapeChars.Push(new EscapeCharacter(CloseCurlyBracket, pos, false));
        }

        private void Validate(
            Stack<InterpolationUnit> units, 
            Stack<EscapeCharacter> escapeChars)
        {
            foreach (var unit in units)
            {
                if (!unit.IsClosed)
                {
                    throw new WrongStringException($"Wrong string starting at {unit.OpenPosition}");
                }
            }

            foreach (var ch in escapeChars)
            {
                if (!ch.IsClosed)
                {
                    throw new WrongStringException($"Wrong string starting at {ch.Position}");
                }
            }
        }
    
        private string ReplaceInterpolationUnits(string template, 
            object target,
            Stack<InterpolationUnit> units, 
            Stack<EscapeCharacter> escapeChars)
        {
            var sb = new StringBuilder(template);
            foreach (var unit in units)
            {
                var name = template.Substring(unit.OpenPosition + 1, unit.ClosePosition - unit.OpenPosition - 1);
                var type = target.GetType();
                Delegate del;
                DictionaryKey key = new() { Type = type, MethodName = name};
                var cacheResult = _cache.TryGetValue(key, out del);

                if (!cacheResult)
                {
                    del = GenerateDelegate(type, name.Trim());
                    _cache.TryAdd(key, del);
                }

                var result = (string)del.DynamicInvoke(target);
                
                sb.Remove(unit.OpenPosition, unit.ClosePosition - unit.OpenPosition + 1);
                sb.Insert(unit.OpenPosition, result);
            }

            sb.Replace("{{", "{");
            sb.Replace("}}", "}");
            return sb.ToString();
        }

        private Delegate GenerateDelegate(Type targetType, string memberName)
        {
            var param = Expression.Parameter(targetType);
            var memberAccess = Expression.MakeMemberAccess(param, targetType.GetMember(memberName).First());
            var methodCall = Expression.Call(memberAccess, targetType.GetMethod("ToString"));
            var delegateType = Expression.GetDelegateType(targetType, typeof(string));
            return Expression.Lambda(delegateType, methodCall, param).Compile();
        }
    }
}