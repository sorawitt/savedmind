using MediatR;

namespace SavedMind.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

public record LoginResult(
    string AccessToken,
    int ExpiresIn,
    string RefreshToken
);