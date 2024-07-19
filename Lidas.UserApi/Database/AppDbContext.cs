using Lidas.UserApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lidas.UserApi.Persist;

public class AppDbContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasData(new Role("Basic"), new Role("Premium"), new Role("Admin"));

            entity.HasKey(role => role.Id);

            entity.HasIndex(role => role.Name).IsUnique();

            entity.Property(role => role.Name).IsRequired().HasMaxLength(200);

            entity.HasMany(role => role.Users)
            .WithOne(user => user.Role);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(user => user.Id);

            entity.HasIndex(user => user.UserName).IsUnique();
            entity.HasIndex(user => user.Email).IsUnique();

            entity.Property(user => user.Name).IsRequired().HasMaxLength(200);
            entity.Property(user => user.LastName).IsRequired().HasMaxLength(200);
            entity.Property(user => user.UserName).IsRequired().HasMaxLength(200);
            entity.Property(user => user.Email).IsRequired().HasMaxLength(250);
            entity.Property(user => user.IsEmailConfirmed).IsRequired();
            entity.Property(user => user.CreatedAt).IsRequired();
            entity.Property(user => user.UpdatedAt).IsRequired();

            entity.HasOne(user => user.Role)
            .WithMany(role => role.Users)
            .IsRequired();
        });
    }

}
