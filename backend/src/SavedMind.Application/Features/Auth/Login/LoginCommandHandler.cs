using MediatR;
using Microsoft.Extensions.Options;
using SavedMind.Application.Common.Exceptions;
using SavedMind.Application.Common.Security;
using SavedMind.Domain.Abstractions;

namespace SavedMind.Application.Features.Auth.Login;

public class LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService, IOptions<JwtSettings> jwtSettings) : IRequestHandler<LoginCommand, LoginResult>
{
    private const int MaxFailedAttempts = 10;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(30);
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            throw new InvalidCredentialsException();
        }
        if (user.IsLockedOut)
        {
            throw new AccountLockedException(user.LockoutEnd!.Value);
        }

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.Add(LockoutDuration);
            }

            await userRepository.UpdateAsync(user, cancellationToken);

            throw new InvalidCredentialsException();
        }

        if (!user.EmailVerified)
        {
            throw new EmailNotVerifiedException(email);
        }

        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await userRepository.UpdateAsync(user, cancellationToken);
        }

        var accessToken = jwtTokenService.GenerateAccessToken(user);
        var refreshToken = await jwtTokenService.GenerateRefreshTokenAsync(user, cancellationToken);

        return new LoginResult(
            AccessToken: accessToken,
            ExpiresIn: jwtSettings.Value.AccessTokenExpiryMinutes * 60,
            RefreshToken: refreshToken
        );
    }
}