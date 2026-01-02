using MediatR;

namespace SavedMind.Application.Features.Auth.Register;

public record RegisterCommand(string Email, string Password) : IRequest<RegisterResult>;

public record RegisterResult(string Message);