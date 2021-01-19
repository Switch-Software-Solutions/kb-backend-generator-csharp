using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoreAuth.Models;

namespace DataAuth.Configurations
{
    public class RecoveryKeyConfiguration : IEntityTypeConfiguration<RecoveryKey>
    {
        public void Configure(EntityTypeBuilder<RecoveryKey> builder)
        {
            builder
                .HasKey(b => b.Id);
            builder
                .Property(b => b.Id)
                .UseIdentityColumn();

            builder
                .Property(b => b.Key)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .Property(b => b.RemoteIpAddress)
                .HasMaxLength(50);

            builder
                .ToTable("RecoveryKeys");
        }
    }
}
