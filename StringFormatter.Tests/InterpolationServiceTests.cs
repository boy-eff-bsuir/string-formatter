using StringFormatter.Core;
using StringFormatter.Core.Exceptions;

namespace StringFormatter.Tests;

public class InterpolationServiceTests
{
    [Theory]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("{{}")]
    [InlineData("{}}")]
    public void Formate_ShouldThrowWrongStringException(string input)
    {
        Formatter formatter = new();
        Assert.Throws<WrongStringException>(() => formatter.Format(input, new object()));
    }
}