using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using StringFormatter.Core.Models;
using StringFormatter.Core.Services;
using StringFormatter.Tests.Fakes;

namespace StringFormatter.Tests
{
    public class CacheServiceTests
    {
        CacheService _sut;
        public CacheServiceTests()
        {
            _sut = new CacheService();
        }

        [Fact]
        public void ShouldGetCachedValue()
        {
            var key = new DictionaryKey() { Type = typeof(FakeClass), MemberName = "TestName", Index = 1 };
            _sut.TryAdd(key, new Action(() => System.Console.WriteLine("Cached")));

            Delegate del;
            var result = _sut.TryGetValue(key, out del);

            result.Should().BeTrue();
        }

        [Fact]
        public void ShouldNotGetCachedValueForDifferentIndex()
        {
            var key = new DictionaryKey() { Type = typeof(FakeClass), MemberName = "TestName", Index = 1 };
            _sut.TryAdd(key, new Action(() => System.Console.WriteLine("Cached")));

            Delegate del;
            key.Index = 2;
            var result = _sut.TryGetValue(key, out del);

            result.Should().BeFalse();
        }

        [Fact]
        public void ShouldNotGetCachedValueForDifferentType()
        {
            var key = new DictionaryKey() { Type = typeof(FakeClass), MemberName = "TestName", Index = 1 };
            _sut.TryAdd(key, new Action(() => System.Console.WriteLine("Cached")));

            Delegate del;
            key.Type = typeof(int);
            var result = _sut.TryGetValue(key, out del);

            result.Should().BeFalse();
        }
    }
}