using SavedMind.Domain.Entities;

namespace SavedMind.Domain.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}
