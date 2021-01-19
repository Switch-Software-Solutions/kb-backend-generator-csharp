using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoreAuth.Models;

namespace DataAuth.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .HasKey(m => m.Id);

            builder
                .Property(m => m.Id)
                .UseIdentityColumn();

            builder
                .Property(m => m.Nick)
                .IsRequired()
                .HasMaxLength(30);

            builder
                .Property(m => m.Salt)
                .HasMaxLength(50);

            builder
                .Property(m => m.PasswordHash)
                .IsRequired()
                .HasMaxLength(100);

            builder
                .Property(m => m.Email)
                .IsRequired()
                .HasMaxLength(60);

            builder
                .HasIndex(u => u.Email).IsUnique();

            builder
                .HasMany(user => user.RefreshTokens)
                .WithOne(token => token.User)
                .HasForeignKey(token => token.UserId);

            builder
                .HasMany(user => user.RecoveryKeys)
                .WithOne(recoveryKey => recoveryKey.User)
                .HasForeignKey(recoveryKey => recoveryKey.UserId);

            builder
                .ToTable("User");
        }
    }
}
