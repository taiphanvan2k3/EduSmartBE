using CourseManagementService.Database.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagementService.Database.Configurations
{
    public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
    {
        public void Configure(EntityTypeBuilder<Bookmark> builder)
        {
            builder.ToTable("Bookmarks");

            builder.HasOne(bookmark => bookmark.Lesson)
                .WithMany(lesson => lesson.Bookmarks)
                .HasForeignKey(bookmark => bookmark.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bookmark => bookmark.Course)
                .WithMany(course => course.Bookmarks)
                .HasForeignKey(bookmark => bookmark.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(bookmark => new { bookmark.UserId, bookmark.LessonId })
                .IsUnique();

            builder.HasIndex(bookmark => new { bookmark.UserId, bookmark.CourseId });
        }
    }
}