using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Databases;

public class ApplicationDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasIndex(u => u.PublicId)
            .IsUnique();

        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.ReplacedByToken)
            .WithOne(rt => rt.ReplacesToken)
            .HasForeignKey<RefreshToken>(rt => rt.ReplacedByTokenId);
    }
}