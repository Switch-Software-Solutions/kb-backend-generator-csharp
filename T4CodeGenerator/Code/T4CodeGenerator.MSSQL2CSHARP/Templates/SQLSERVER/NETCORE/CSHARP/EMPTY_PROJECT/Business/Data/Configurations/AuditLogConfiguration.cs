
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Models;

namespace Data.Configurations
{
    public partial class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {

            builder.HasKey(auditlog => auditlog.Id); builder.Property(auditlog => auditlog.Id).IsRequired().UseIdentityColumn();

            builder.Property(auditlog => auditlog.Token).IsRequired().HasMaxLength(500);

            builder.Property(auditlog => auditlog.EventDate).IsRequired();

            builder.Property(auditlog => auditlog.EventType).IsRequired().HasMaxLength(50);

            builder.Property(auditlog => auditlog.TableName).IsRequired().HasMaxLength(50);

            builder.Property(auditlog => auditlog.RecordValues).IsRequired();

            builder.Property(auditlog => auditlog.TimeStamp).IsRowVersion();

            builder.Property(auditlog => auditlog.HashValue).IsRequired();

            builder
                .ToTable("AuditLog");
        }
    }
}
