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
        }
    }
}