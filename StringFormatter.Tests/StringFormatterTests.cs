using FluentAssertions;
using StringFormatter.Core;
using StringFormatter.Core.Exceptions;
using StringFormatter.Core.Services;
using StringFormatter.Tests.Fakes;

namespace StringFormatter.Tests;

public class StringFormatterTests
{
    private Formatter _sut;
    public StringFormatterTests()
    {
        CacheService cacheService = new();
        ValidationService validationService = new();
        _sut = new Formatter(cacheService, validationService);
    }

    [Fact]
    public void Format_ShouldSucceed()
    {
        FakeClass fake = new() { Age = 10 };
        var input = "{{Age}} is {Age}";
        var expected = "{Age} is 10";

        var result = _sut.Format(input, fake);

        result.Should().NotBeNullOrEmpty();
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Format_ShouldSucceedWithMultipleInterpolationUnits()
    {
        FakeClass fake = new()
        {
            Name = "Danila",
            Age = 20
        };

        var input = "{{Age}} is {Age}, {{Name}} is {Name}";
        var expected = "{Age} is 20, {Name} is Danila";

        var result = _sut.Format(input, fake);

        result.Should().NotBeNullOrEmpty();
        result.Should().BeEquivalentTo(expected);
    }
}