using CourseManagementService.Database.Schemas.DiscussionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagementService.Database.Configurations
{
    public class DiscussionConfiguration : IEntityTypeConfiguration<Discussion>
    {
        public void Configure(EntityTypeBuilder<Discussion> builder)
        {
            builder.Property(d => d.Title)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(d => d.Content)
                .IsRequired();

            builder.HasOne(d => d.Lesson)
                .WithMany(l => l.Discussions)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(d => d.Course)
                .WithMany(c => c.Discussions)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(d => new { d.CourseId, d.CreatedBy });
        }
    }
}