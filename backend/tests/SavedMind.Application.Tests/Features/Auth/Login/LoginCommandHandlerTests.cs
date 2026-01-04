using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using SavedMind.Application.Common.Exceptions;
using SavedMind.Application.Common.Security;
using SavedMind.Application.Features.Auth.Login;
using SavedMind.Domain.Abstractions;
using SavedMind.Domain.Entities;

namespace SavedMind.Application.Tests.Features.Auth.Login;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IOptions<JwtSettings> _jwtSettings;

    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTests()
    {
        // Create mock
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtTokenService = Substitute.For<IJwtTokenService>();

        // Setup options
        _jwtSettings = Options.Create(new JwtSettings
        {
            AccessTokenExpiryMinutes = 15
        });

        // Create handler with mocks
        _sut = new LoginCommandHandler(_userRepository, _passwordHasher, _jwtTokenService, _jwtSettings);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsTokens()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "password123");
        var user = CreateVerifiedUser();

        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("password123", user.PasswordHash).Returns(true);
        _jwtTokenService.GenerateAccessToken(user).Returns("access-token");
        _jwtTokenService.GenerateRefreshTokenAsync(user, Arg.Any<CancellationToken>()).Returns("refresh-token");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.ExpiresIn.Should().Be(900);
    }

    [Fact]
    public async Task Handle_InvalidEmail_ThrowsInvalidCredentials()
    {
        // Arrange
        var command = new LoginCommand("notfound@example.com", "Password123");

        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task Handle_InvalidPassword_ThrowsInvalidCredentials()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "WrongPassword");
        var user = CreateVerifiedUser();

        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.Verify("WrongPassword", user.PasswordHash)
            .Returns(false);

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task Handle_LockedAccount_ThrowsAccountLocked()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123");
        var user = CreateVerifiedUser();
        user.LockoutEnd = DateTime.UtcNow.AddMinutes(30); // Locked for 30 min

        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AccountLockedException>();
    }

    [Fact]
    public async Task Handle_UnverifiedEmail_ThrowsEmailNotVerified()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123");
        var user = CreateVerifiedUser();
        user.EmailVerified = false;

        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.Verify("Password123", user.PasswordHash)
            .Returns(true);

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EmailNotVerifiedException>();
    }

    [Fact]
    public async Task Handle_InvalidPassword_IncrementsFailedAttempts()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "WrongPassword");
        var user = CreateVerifiedUser();
        user.FailedLoginAttempts = 5;

        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.Verify("WrongPassword", user.PasswordHash)
            .Returns(false);

        // Act
        try { await _sut.Handle(command, CancellationToken.None); }
        catch (InvalidCredentialsException) { }

        // Assert
        user.FailedLoginAttempts.Should().Be(6);
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TenthFailedAttempt_LocksAccount()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "WrongPassword");
        var user = CreateVerifiedUser();
        user.FailedLoginAttempts = 9; // This will be the 10th attempt

        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.Verify("WrongPassword", user.PasswordHash)
            .Returns(false);

        // Act
        try { await _sut.Handle(command, CancellationToken.None); }
        catch (InvalidCredentialsException) { }

        // Assert
        user.FailedLoginAttempts.Should().Be(10);
        user.LockoutEnd.Should().NotBeNull();
        user.LockoutEnd.Should().BeCloseTo(
            DateTime.UtcNow.AddMinutes(30),
            TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_SuccessfulLogin_ResetsFailedAttempts()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123");
        var user = CreateVerifiedUser();
        user.FailedLoginAttempts = 5;

        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.Verify("Password123", user.PasswordHash)
            .Returns(true);
        _jwtTokenService.GenerateAccessToken(user)
            .Returns("access-token");
        _jwtTokenService.GenerateRefreshTokenAsync(user, Arg.Any<CancellationToken>())
            .Returns("refresh-token");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        user.FailedLoginAttempts.Should().Be(0);
        await _userRepository.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    // Helper method
    private static User CreateVerifiedUser() => new()
    {
        Id = Guid.NewGuid(),
        Email = "test@example.com",
        PasswordHash = "hashed_password",
        EmailVerified = true,
        FailedLoginAttempts = 0,
        LockoutEnd = null
    };

}