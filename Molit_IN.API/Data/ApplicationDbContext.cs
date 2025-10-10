// API/Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using Molit_IN.API.Models;

namespace Molit_IN.API.Data
{

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
    { }
        public DbSet<User> Users { get; set; }
        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<Role> Roles => Set<Role>();
    }

}
