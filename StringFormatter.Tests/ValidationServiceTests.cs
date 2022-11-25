using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using StringFormatter.Core.Dtos;
using StringFormatter.Core.Models;
using StringFormatter.Core.Services;

namespace StringFormatter.Tests
{
    public class ValidationServiceTests
    {
        ValidationService _sut;

        public ValidationServiceTests()
        {
            _sut = new ValidationService();
        }

        [Fact]
        public void ValidateEscapeCharacter_ShouldSucceedIfCharIsClosed()
        {
            var escapeChar = new EscapeCharacter('{', 0, true);

            var result = _sut.ValidateEscapeCharacter(escapeChar);

            result.Should().NotBeNull();
            result.Succeed.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public void ValidateEscapeCharacter_ShouldFailIfCharIsNotClosed()
        {
            var escapeChar = new EscapeCharacter('{', 0, false);

            var result = _sut.ValidateEscapeCharacter(escapeChar);

            result.Should().NotBeNull();
            result.Succeed.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void ValidateInterpolationUnit_ShouldSucceedIfUnitIsClosed()
        {
            var unit = new InterpolationUnitDto(0)
            {
                CloseCurlyBracketPosition = 1,
            };

            var result = _sut.ValidateInterpolationUnit(unit);

            result.Should().NotBeNull();
            result.Succeed.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public void ValidateInterpolationUnit_ShouldFailIfUnitIsNotClosed()
        {
            var unit = new InterpolationUnitDto(0);
            
            var result = _sut.ValidateInterpolationUnit(unit);

            result.Should().NotBeNull();
            result.Succeed.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void ValidateInterpolationUnit_ShouldFailIfUnitIsNotClosedArray()
        {
            var unit = new InterpolationUnitDto(0)
            {
                CloseCurlyBracketPosition = 3,
                OpenSquareBracketPosition = 1
            };
            
            var result = _sut.ValidateInterpolationUnit(unit);

            result.Should().NotBeNull();
            result.Succeed.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }
    }
}