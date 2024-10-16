using CourseManagementService.Database.Schemas;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database
{
    public static class ModelCreate
    {
        public static ModelBuilder OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");
                entity.HasKey(e => e.Id);

                entity.HasMany(c => c.Chapters)
                    .WithOne(ch => ch.Course)
                    .HasForeignKey(ch => ch.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Tags)
                    .WithOne(ct => ct.Course)
                    .HasForeignKey(ct => ct.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(c => c.Type)
                    .HasConversion<string>()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tags");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<CourseTag>(entity =>
            {
                entity.ToTable("CourseTags");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(e => e.Id);

                entity.HasMany(c => c.Courses)
                    .WithOne(c => c.Category)
                    .HasForeignKey(c => c.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.ToTable("Chapters");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasMany(c => c.Courses)
                    .WithOne(c => c.Currency)
                    .HasForeignKey(c => c.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CourseEnrollment>(entity =>
            {
                entity.ToTable("CourseEnrollments");
                entity.HasKey(e => new { e.CourseId, e.StudentId });

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Enrollments)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            return modelBuilder;
        }
    }
}