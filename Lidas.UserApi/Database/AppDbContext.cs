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
     
}
