using StringFormatter.Core;
using StringFormatter.Core.Exceptions;
using StringFormatter.Core.Services;
using StringFormatter.Tests.Fakes;

namespace StringFormatter.Tests;

public class InterpolationServiceTests
{
    private Formatter _sut;
    public InterpolationServiceTests()
    {
        CacheService cacheService = new();
        ValidationService validationService = new();
        _sut = new Formatter(cacheService, validationService);
    }

    [Theory]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("{{}")]
    [InlineData("{}}")]
    public void Format_ShouldThrowWrongStringException(string input)
    {
        Assert.Throws<WrongStringException>(() => _sut.Format(input, new object()));
    }

    [Fact]
    public void Format_ShouldReturnSuccess()
    {
        FakeClass fake = new();
        var input = "{{Age}} is {Age}, {{Child}} is {Children[0]}";
        var age = 10;
        var expected = "{Age} is 10, {Child} is Suzy";

        var result = _sut.Format(input, new FakeClass() { Age = age });

        Assert.Equal(expected, result);
    }
}