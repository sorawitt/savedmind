using MediatR;
using SavedMind.Application.Common.Exceptions;
using SavedMind.Application.Common.Security;
using SavedMind.Domain.Abstractions;
using SavedMind.Domain.Entities;

namespace SavedMind.Application.Features.Auth.Register;

public class RegisterCommandHandler(
    IUserRepository users,
    IEmailVerificationTokenRepository tokens,
    IPasswordHasher passwordHasher,
    IEmailService emailService) : IRequestHandler<RegisterCommand, RegisterResult>
{
    // Token expiration time (could be moved to configuration)
    private static readonly TimeSpan TokenExpiration = TimeSpan.FromHours(24);

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Normalize email (lowercase, trimmed)
        var email = request.Email.Trim().ToLowerInvariant();

        // Step 2: Check for existing user
        var existingUser = await users.GetByEmailAsync(email, cancellationToken);
        if (existingUser is not null)
        {
            throw new DuplicateEmailException(email);
        }

        // Step 3: Hash password with BCrypt
        var passwordHash = passwordHasher.Hash(request.Password);

        // Step 4: Create user entity
        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            EmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        await users.AddAsync(user, cancellationToken);

        // Step 5: Generate verification token
        var rawToken = TokenGenerator.GenerateSecureToken();
        var tokenHash = TokenGenerator.ComputeSha256Hash(rawToken);

        var verificationToken = new EmailVerificationToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.Add(TokenExpiration),
            IsUsed = false
        };

        await tokens.AddAsync(verificationToken, cancellationToken);

        // Step 6: Build verification link and send email
        // TODO: Move base URL to configuration (IConfiguration or IOptions<AppSettings>)
        var verificationLink = $"https://localhost:5001/api/auth/verify?token={Uri.EscapeDataString(rawToken)}&userId={user.Id}";

        await emailService.SendVerificationEmailAsync(user.Email, verificationLink, cancellationToken);

        // Step 7: Return success
        return new RegisterResult("Registration successful. Please check your email.");
    }
}