// EntraUser.Infrastructure/Data/AppDbContext.cs
namespace EntraUser.Infrastructure.Data;

using EntraUser.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<UserSession>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.UserUpn).IsUnique();
            e.Property(x => x.UserUpn).IsRequired().HasMaxLength(256);
            e.Property(x => x.DisplayName).HasMaxLength(256);
            e.Property(x => x.ObjectId).HasMaxLength(128);
            e.Property(x => x.PinHash).HasMaxLength(512);
            e.Property(x => x.AccessToken).HasMaxLength(4096);
            e.Property(x => x.RefreshToken).HasMaxLength(4096);
            e.Property(x => x.HasPin).HasDefaultValue(false);
            e.Property(x => x.PasswordChangeRequired).HasDefaultValue(true);
        });
    }
}
