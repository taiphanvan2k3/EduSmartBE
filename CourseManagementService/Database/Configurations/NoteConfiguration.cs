using CourseManagementService.Database.Schemas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseManagementService.Database.Configurations
{
    public class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.ToTable("Notes");

            builder.Property(note => note.LessonType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasOne(note => note.Lesson)
                .WithMany(lesson => lesson.Notes)
                .HasForeignKey(note => note.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(note => note.Chapter)
                .WithMany(chapter => chapter.Notes)
                .HasForeignKey(note => note.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}