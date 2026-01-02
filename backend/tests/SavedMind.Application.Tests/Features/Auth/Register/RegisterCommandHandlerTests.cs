using FluentAssertions;
using NSubstitute;
using SavedMind.Application.Common.Exceptions;
using SavedMind.Application.Features.Auth.Register;
using SavedMind.Domain.Abstractions;
using SavedMind.Domain.Entities;

namespace SavedMind.Application.Tests.Features.Auth.Register;

public class RegisterCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationTokenRepository _tokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly RegisterCommandHandler _sut;

    public RegisterCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _tokenRepository = Substitute.For<IEmailVerificationTokenRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _emailService = Substitute.For<IEmailService>();

        _sut = new RegisterCommandHandler(
            _userRepository,
            _tokenRepository,
            _passwordHasher,
            _emailService);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessMessage()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Password123");
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed_password");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Contain("successful");
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesUser()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Password123");
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed_password");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        await _userRepository.Received(1).AddAsync(
            Arg.Is<User>(u => u.Email == "test@example.com"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidRequest_HashesPassword()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Password123");
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        _passwordHasher.Hash("Password123").Returns("hashed_password");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        _passwordHasher.Received(1).Hash("Password123");
        await _userRepository.Received(1).AddAsync(
            Arg.Is<User>(u => u.PasswordHash == "hashed_password"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesVerificationToken()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Password123");
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed_password");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        await _tokenRepository.Received(1).AddAsync(
            Arg.Is<EmailVerificationToken>(t => t.TokenHash != null && !t.IsUsed),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidRequest_SendsVerificationEmail()
    {
        // Arrange
        var command = new RegisterCommand("test@example.com", "Password123");
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed_password");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        await _emailService.Received(1).SendVerificationEmailAsync(
            "test@example.com",
            Arg.Is<string>(link => link.Contains("verify")),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsDuplicateEmailException()
    {
        // Arrange
        var command = new RegisterCommand("existing@example.com", "Password123");
        var existingUser = new User { Email = "existing@example.com" };
        _userRepository.GetByEmailAsync("existing@example.com", Arg.Any<CancellationToken>())
            .Returns(existingUser);

        // Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DuplicateEmailException>()
            .WithMessage("*existing@example.com*");
    }

    [Fact]
    public async Task Handle_ValidRequest_NormalizesEmail()
    {
        // Arrange
        var command = new RegisterCommand("  TEST@EXAMPLE.COM  ", "Password123");
        _userRepository.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed_password");

        // Act
        await _sut.Handle(command, CancellationToken.None);

        // Assert
        await _userRepository.Received(1).AddAsync(
            Arg.Is<User>(u => u.Email == "test@example.com"),
            Arg.Any<CancellationToken>());
    }
}
