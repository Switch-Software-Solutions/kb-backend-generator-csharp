using Microsoft.EntityFrameworkCore;
using Core.Models;
using Data.Configurations;

namespace Data
{
    public partial class DbContextInstance : DbContext
    {
        public DbSet<AuditLog> AuditLogSet { get; set; }

        public DbContextInstance(DbContextOptions<DbContextInstance> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AuditLogConfiguration());
        }
    }
}

