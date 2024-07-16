using Lidas.MangaApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lidas.MangaApi.Persist;

public class AppDbContext: DbContext
{
    public DbSet<Manga> Mangas { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<Role> Roles { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Manga>(entity =>
        {
            entity.HasKey(manga => manga.Id);

            entity.Property(entity => entity.Banner).IsRequired();
            entity.Property(entity => entity.Cover).IsRequired();
            entity.Property(entity => entity.Name).IsRequired().HasMaxLength(200);
            entity.Property(entity => entity.Description).IsRequired().HasMaxLength(500);
            entity.Property(entity => entity.Release).IsRequired();

            entity.HasMany(entity => entity.Authors).WithMany(entity => entity.Mangas);
            entity.HasMany(entity => entity.Categories).WithMany(entity => entity.Mangas);
            entity.HasMany(entity => entity.Chapters).WithOne();
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(author => author.Id);

            entity.Property(entity => entity.Name).IsRequired().HasMaxLength(200);
            entity.Property(entity => entity.Biography).IsRequired();
            entity.Property(entity => entity.Birthday).IsRequired();

            entity.HasMany(entity => entity.Roles).WithMany(entity => entity.Authors);
            entity.HasMany(entity => entity.Mangas).WithMany(entity => entity.Authors);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(category => category.Id);

            entity.Property(entity => entity.Name).IsRequired().HasMaxLength(200);

            entity.HasMany(entity => entity.Mangas).WithMany(entity => entity.Categories);
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(chapter => chapter.Id);

            entity.Property(entity => entity.Number).IsRequired();
            entity.Property(entity => entity.Title).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(role => role.Id);

            entity.Property(entity => entity.Name).IsRequired().HasMaxLength(200);

            entity.HasMany(entity => entity.Authors).WithMany(entity => entity.Roles);
        });
    }
}
