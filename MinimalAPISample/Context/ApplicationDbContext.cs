using Microsoft.EntityFrameworkCore;
using MinimalAPISample.Entity;

namespace MinimalAPISample.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
