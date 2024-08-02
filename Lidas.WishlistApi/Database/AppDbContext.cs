using Lidas.WishlistApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lidas.WishlistApi.Database;

public class AppDbContext: DbContext
{
    public DbSet<Wishlist> Wishlists { get; set; }
    public DbSet<WishItem> Wishitems { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(list => list.Id);

            entity.Property(list => list.UserId).IsRequired();

            entity.HasMany(list => list.Wishitems).WithMany(item => item.Wishlists);
        });

        modelBuilder.Entity<WishItem>(entity =>
        {
            entity.HasKey(item => item.Id);

            entity.Property(item => item.MangaId).IsRequired();

            entity.HasMany(item => item.Wishlists).WithMany(list => list.Wishitems);
        });
    }
}
