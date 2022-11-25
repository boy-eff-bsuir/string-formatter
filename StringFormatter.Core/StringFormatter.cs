using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using StringFormatter.Core.Dtos;
using StringFormatter.Core.Exceptions;
using StringFormatter.Core.Extensions;
using StringFormatter.Core.Interfaces;
using StringFormatter.Core.Models;

namespace StringFormatter.Core
{
    public class Formatter : IStringFormatter
    {
        private const char OpenCurlyBracket = '{';
        private const char CloseCurlyBracket = '}';
        private const char OpenSquareBracket = '[';
        private const char CloseSquareBracket = ']';
        private readonly ICacheService _cacheService;
        private readonly IValidationService _validationService;

        public Formatter(ICacheService cacheService, IValidationService validationService)
        {
            _cacheService = cacheService;
            _validationService = validationService;
        }

        public string Format(string template, object target)
        {
            Stack<InterpolationUnitDto> units = new();
            Stack<EscapeCharacter> escapeChars = new();
            ProcessInputString(template, units, escapeChars);
            Validate(units, escapeChars);
            
            var result = GenerateInterpolatedString(template, target, units);
            return result.ToString();
        }

        private void ProcessInputString(string template,
            Stack<InterpolationUnitDto> units, 
            Stack<EscapeCharacter> escapeChars)
        {
            for (int i = 0; i < template.Length; i++)
            {
                switch (template[i])
                {
                    case OpenCurlyBracket:
                    {
                        ProcessOpenCurlyBracket(i, units, escapeChars);
                        break;
                    }

                    case CloseCurlyBracket:
                    {
                        ProcessCloseCurlyBracket(i, units, escapeChars);
                        break;
                    }

                    case OpenSquareBracket:
                    {
                        ProcessOpenSquareBracket(i, units);
                        break;
                    }

                    case CloseSquareBracket:
                    {
                        ProcessCloseSquareBracket(i, units);
                        break;
                    }

                }
            }
        }

        private void ProcessOpenCurlyBracket(int pos,
            Stack<InterpolationUnitDto> units, 
            Stack<EscapeCharacter> escapeChars)
        {
            if (units.Any() && units.Peek().OpenCurlyBracketPosition == pos - 1)
            {
                escapeChars.Push(new(OpenCurlyBracket, pos - 1, true));
                units.Pop();
            }
            else
            {
                units.Push(new InterpolationUnitDto(pos));
            }
        }

        private void ProcessCloseCurlyBracket(int pos,
            Stack<InterpolationUnitDto> units, 
            Stack<EscapeCharacter> escapeChars)
        {
            if (units.Any() && !units.Peek().IsClosed)
            {
                units.Peek().CloseCurlyBracketPosition = pos;
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

        private void ProcessOpenSquareBracket(int pos, Stack<InterpolationUnitDto> units)
        {
            var unit = units.Peek();
            if (unit.IsClosed)
            {
                return;
            }

            if (unit.OpenSquareBracketPosition != -1)
            {
                throw new WrongStringException($"Double square bracket at pos {pos}");
            }

            unit.OpenSquareBracketPosition = pos;
        }

        private void ProcessCloseSquareBracket(int pos, Stack<InterpolationUnitDto> units)
        {
            var unit = units.Peek();
            if (unit.IsClosed)
            {
                return;
            }

            if (unit.OpenSquareBracketPosition == -1)
            {
                throw new WrongStringException($"Close square bracket at pos {pos}");
            }

            if (unit.CloseSquareBracketPosition != -1)
            {
                throw new WrongStringException($"Double square bracket at pos {pos}");
            }

            unit.CloseSquareBracketPosition = pos;
        }

        private void Validate(
            Stack<InterpolationUnitDto> units, 
            Stack<EscapeCharacter> escapeChars)
        {
            foreach (var unit in units)
            {
                var result = _validationService.ValidateInterpolationUnit(unit);
                if (!result.Succeed)
                {
                    throw new WrongStringException(result.ErrorMessage);
                }
            }

            foreach (var ch in escapeChars)
            {
                var result = _validationService.ValidateEscapeCharacter(ch);
                if (!result.Succeed)
                {
                    throw new WrongStringException(result.ErrorMessage);
                }
            }
        }
    
        private StringBuilder GenerateInterpolatedString(string template, 
            object target,
            Stack<InterpolationUnitDto> units)
        {
            var sb = new StringBuilder(template);
            ProcessInterpolationUnits(sb, target, units, template);
            ProcessEscapeCharacters(sb);
            return sb;
        }

        private void ProcessInterpolationUnits(StringBuilder sb,
            object target, 
            Stack<InterpolationUnitDto> units, 
            string template)
        {
            var type = target.GetType();
            foreach (var dto in units)
            {
                var unit = dto.ToUnit(template);
                
                Delegate del;
                DictionaryKey key = new() { Type = type, MemberName = unit.Name, Index = unit.Index};
                var cacheResult = _cacheService.TryGetValue(key, out del);

                if (!cacheResult)
                {
                    if (unit.IsArray)
                    {
                        del = GenerateArrayDelegate(type, unit.Name, unit.Index);
                    }
                    else
                    {
                        del = GenerateDelegate(type, unit.Name);
                    }
                    
                    _cacheService.TryAdd(key, del);
                }

                
                string result;
                if (unit.IsArray)
                {
                    result = (string)del.DynamicInvoke(target);
                }
                else
                {
                    result = (string)del.DynamicInvoke(target);
                }
                
                sb.Remove(dto.OpenCurlyBracketPosition, 
                    dto.CloseCurlyBracketPosition - dto.OpenCurlyBracketPosition + 1);
                sb.Insert(dto.OpenCurlyBracketPosition, result);
            }
        }

        private void ProcessEscapeCharacters(StringBuilder sb)
        {
            sb.Replace("{{", "{");
            sb.Replace("}}", "}");
        }
        private Delegate GenerateDelegate(Type targetType, string memberName)
        {
            var targetTypeParameter = Expression.Parameter(targetType);
            var memberInfo = targetType.GetMember(memberName).First();
            var memberAccess = Expression.MakeMemberAccess(targetTypeParameter, memberInfo);
            var methodCall = Expression.Call(memberAccess, memberInfo.DeclaringType.GetMethod("ToString"));
            var delegateType = Expression.GetDelegateType(targetType, typeof(string));
            return Expression.Lambda(delegateType, methodCall, targetTypeParameter).Compile();
        }

        private Delegate GenerateArrayDelegate(Type targetType, string memberName, int index)
        {
            var targetTypeParameter = Expression.Parameter(targetType);
            var memberInfo = targetType.GetMember(memberName).First();
            var arrayElementInfo = memberInfo.GetUnderlyingType().GetElementType();
            var memberAccess = Expression.MakeMemberAccess(targetTypeParameter, memberInfo);
            var arrayAccess = Expression.ArrayIndex(memberAccess, Expression.Constant(index));
            //var arrayAccess = Expression.ArrayAccess(memberAccess, indexParameter);
            var method = arrayElementInfo.GetMethod("ToString", new Type[0]);
            var methodCall = Expression.Call(arrayAccess, method);
            var delegateType = Expression.GetDelegateType(targetType, typeof(string));
            return Expression.Lambda(delegateType, methodCall, targetTypeParameter).Compile();
        }
    }
}