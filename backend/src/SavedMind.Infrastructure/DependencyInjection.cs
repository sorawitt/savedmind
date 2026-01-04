using Microsoft.Extensions.DependencyInjection;
using SavedMind.Application.Common.Security;
using SavedMind.Domain.Abstractions;
using SavedMind.Infrastructure.Persistence.Repositories;
using SavedMind.Infrastructure.Security;
using SavedMind.Infrastructure.Services;

namespace SavedMind.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Security
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Services  
        services.AddScoped<IEmailService, EmailService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        return services;
    }
}