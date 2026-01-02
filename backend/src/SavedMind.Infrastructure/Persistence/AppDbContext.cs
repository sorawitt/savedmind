using Microsoft.EntityFrameworkCore;
using SavedMind.Domain.Entities;

namespace SavedMind.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("vector");

        // =========================================================================
        // MANUAL SNAKE_CASE NAMING CONVENTION
        // =========================================================================
        // Why manual? EFCore.NamingConventions package (v9.0.0) does not support
        // .NET 10 / EF Core 10.x yet. Once it's updated, we can replace all of this
        // with a single line: .UseSnakeCaseNamingConvention() in Program.cs
        //
        // TODO: Remove manual column names when EFCore.NamingConventions supports EF Core 10
        // GitHub: https://github.com/efcore/EFCore.NamingConventions
        // =========================================================================

        // User Entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Bookmark Entity
        modelBuilder.Entity<Bookmark>(entity =>
        {
            entity.ToTable("bookmarks");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Embedding).HasColumnName("embedding").HasColumnType("vector(1536)");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasIndex(e => e.Url);
        });

        // Topic Entity
        modelBuilder.Entity<Topic>(entity =>
        {
            entity.ToTable("topics");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        // RefreshToken Entity
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_tokens");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
        });

        // EmailVerificationToken Entity
        modelBuilder.Entity<EmailVerificationToken>(entity =>
        {
            entity.ToTable("email_verification_tokens");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.TokenHash).HasColumnName("token_hash");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.IsUsed).HasColumnName("is_used");
        });

        // Many-to-Many: Bookmark <-> Topic
        modelBuilder.Entity<Bookmark>()
            .HasMany(b => b.Topics)
            .WithMany(t => t.Bookmarks)
            .UsingEntity(j => j.ToTable("bookmark_topics"));
    }
}
