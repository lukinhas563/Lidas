using Lidas.WishlistApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lidas.WishlistApi.Database;

public class AppDbContext: DbContext
{
    public DbSet<Wish> Wishes { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }
}
