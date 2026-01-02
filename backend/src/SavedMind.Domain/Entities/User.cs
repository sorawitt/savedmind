using System.ComponentModel.DataAnnotations;

namespace SavedMind.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool EmailVerified { get; set; } = false;

    // Navigation Properties (EF Core Relationships)
    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}