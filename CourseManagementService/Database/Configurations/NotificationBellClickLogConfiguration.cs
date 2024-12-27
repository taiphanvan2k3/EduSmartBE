using CourseManagementService.Database.Schemas.NotificationEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagementService.Database.Configurations
{
    public class NotificationBellClickLogConfiguration : IEntityTypeConfiguration<NotificationBellClickLog>
    {
        public void Configure(EntityTypeBuilder<NotificationBellClickLog> builder)
        {
            builder.ToTable("NotificationBellClickLogs");

            builder.HasIndex(log => log.UserId)
                .IsUnique();

            builder.Property(log => log.LastClickedAt)
                .IsRequired();
        }
    }
}