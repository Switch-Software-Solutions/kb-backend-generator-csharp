using Microsoft.EntityFrameworkCore;
using CoreAuth.Models;
using DataAuth.Configurations;

namespace DataAuth
{
    public partial class AuthDbContext : DbContext
    {
        public DbSet<User> UserSet { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .ApplyConfiguration(new UserConfiguration());
        }
    }
}
