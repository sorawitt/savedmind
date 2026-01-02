using Microsoft.EntityFrameworkCore;
using SavedMind.Domain.Abstractions;
using SavedMind.Domain.Entities;

namespace SavedMind.Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct)
    {
        await context.Users.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);
    }
}