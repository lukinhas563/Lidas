using LikesApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LikesApi.Database;

public class AppDbContext: DbContext
{
    public DbSet<Likelist> Likelists { get; set; }
    public DbSet<Likeitem> Likeitems { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Likelist>(entity =>
        {
            entity.HasKey(list => list.Id);

            entity.Property(list => list.UserId).IsRequired();

            entity.HasMany(list => list.LikeItems).WithMany(item => item.LikeLists);
        });

        modelBuilder.Entity<Likeitem>(entity =>
        {
            entity.HasKey(item => item.Id);

            entity.Property(item => item.MangaId).IsRequired();

            entity.HasMany(item => item.LikeLists).WithMany(list => list.LikeItems);
        });
    }
}
