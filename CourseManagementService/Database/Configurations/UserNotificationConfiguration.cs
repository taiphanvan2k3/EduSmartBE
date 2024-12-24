using CourseManagementService.Database.Schemas.NotificationEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace CourseManagementService.Database.Configurations
{
    public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.ToTable("UserNotifications");

            builder.HasIndex(notification => new { notification.ReceiverId, notification.CourseId });

            builder.Property(notification => notification.Type)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(notification => notification.MetaData)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(notification => notification.RelatedEntityId)
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(notification => notification.RelatedEntityType)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(notification => notification.Course)
                .WithMany()
                .HasForeignKey(notification => notification.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(notification => notification.SenderInfo)
                .HasColumnType("jsonb")
                .HasConversion(
                    senderInfo => JsonConvert.SerializeObject(senderInfo),
                    senderInfo => JsonConvert.DeserializeObject<SenderInfo>(senderInfo)
                );
        }
    }
}