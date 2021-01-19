using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoreAuth.Models;

namespace DataAuth.Configurations
{
    public class RefreshTokenConfigurations : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder
                .HasKey(b => b.Id);
            builder
                .Property(b => b.Id)
                .UseIdentityColumn();

            builder
                .Property(b => b.Token)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .Property(b => b.RemoteIpAddress)
                .HasMaxLength(50);

            builder
                .ToTable("RefreshTokens");
        }
    }
}
