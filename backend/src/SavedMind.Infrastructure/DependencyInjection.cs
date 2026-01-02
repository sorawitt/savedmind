using Microsoft.Extensions.DependencyInjection;
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

        // Services  
        services.AddScoped<IEmailService, EmailService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();

        return services;
    }
}