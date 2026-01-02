using FluentAssertions;
using SavedMind.Application.Features.Auth.Register;

namespace SavedMind.Application.Tests.Features.Auth.Register;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _sut;

    public RegisterCommandValidatorTests()
    {
        _sut = new RegisterCommandValidator();
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user@domain.org", true)]
    [InlineData("name.surname@company.co.th", true)]
    public async Task Validate_ValidEmail_NoErrors(string email, bool shouldBeValid)
    {
        // Arrange
        var command = new RegisterCommand(email, "ValidPass123");

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().Be(shouldBeValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    public async Task Validate_InvalidEmail_ReturnsError(string email)
    {
        // Arrange
        var command = new RegisterCommand(email, "ValidPass123");

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task Validate_EmptyPassword_ReturnsError()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "");

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Theory]
    [InlineData("Short1")]
    [InlineData("Abc123")]
    [InlineData("1234567")]
    public async Task Validate_ShortPassword_ReturnsError(string password)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", password);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" && 
            e.ErrorMessage.Contains("8"));
    }

    [Theory]
    [InlineData("NoNumbersHere")]
    [InlineData("ALLUPPERCASE")]
    public async Task Validate_PasswordNoNumber_ReturnsError(string password)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", password);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" && 
            e.ErrorMessage.Contains("number"));
    }

    [Theory]
    [InlineData("alllowercase123")]
    [InlineData("nouppercase1")]
    public async Task Validate_PasswordNoUppercase_ReturnsError(string password)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", password);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "Password" && 
            e.ErrorMessage.Contains("uppercase"));
    }

    [Theory]
    [InlineData("ValidPass1")]
    [InlineData("Password123")]
    [InlineData("MySecure1Password")]
    public async Task Validate_ValidPassword_NoErrors(string password)
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", password);

        // Act
        var result = await _sut.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
