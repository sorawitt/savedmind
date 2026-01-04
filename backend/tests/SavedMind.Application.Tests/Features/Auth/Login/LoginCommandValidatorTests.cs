using FluentAssertions;
using SavedMind.Application.Features.Auth.Login;

namespace SavedMind.Application.Tests.Features.Auth.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _sut;

    public LoginCommandValidatorTests()
    {
        _sut = new LoginCommandValidator();
    }

    [Theory]
    [InlineData("test@example.com", "Password123", true)]
    [InlineData("user@domain.org", "Secure1Pass", true)]
    public async Task Validate_ValidInput_NoErrors(string email, string password, bool expected)
    {
        var command = new LoginCommand(email, password);
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid")]
    [InlineData("@domain.com")]
    public async Task Validate_InvalidEmail_ReturnsError(string email)
    {
        var command = new LoginCommand(email, "Password123");
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_EmptyPassword_ReturnsError(string password)
    {
        var command = new LoginCommand("test@example.com", password);
        var result = await _sut.ValidateAsync(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }
}