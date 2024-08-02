using Lidas.LikeApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lidas.LikeApi.Database;

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

            entity.HasMany(list => list.Likeitems).WithMany(item => item.Likelists);
        });

        modelBuilder.Entity<Likeitem>(entity =>
        {
            entity.HasKey(item => item.Id);

            entity.Property(item => item.MangaId).IsRequired();

            entity.HasMany(item => item.Likelists).WithMany(list => list.Likeitems);
        });
    }
}
