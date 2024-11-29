using System.Reflection;
using CourseManagementService.Database.Schemas;
using CourseManagementService.Database.Schemas.DiscussionEntities;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database
{
    // Mặc định User của Identity sử dụng key kiểu string, ta cần custom lại để sử dụng key kiểu int
    public class DataContext : DbContext
    {
        // Không sử dụng nhiều constructor, sẽ gây ra lỗi khi dùng AddDbContextPool
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ModelCreate.OnModelCreating(modelBuilder);

            // Sử dụng các file cấu hình riêng cho từng entity
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            ModelCreate.ConfigureForBaseEntity(modelBuilder);
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CourseTag> CourseTags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<QuizLesson> QuizLessons { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }
        public DbSet<VideoLesson> VideoLessons { get; set; }
        public DbSet<CourseRating> CourseRatings { get; set; }
        public DbSet<LessonRating> LessonRatings { get; set; }
        public DbSet<LessonTracking> LessonTrackings { get; set; }
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<DiscussionType> DiscussionTypes { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Because PostgreSQL does not allow DateTimeOffsets with non-UTC time zones, we need to convert all DateTimeOffsets 
        /// to UTC before saving them to the database.
        /// </summary>
        private void UpdateTimestamps()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                // Kiểm tra entity có CreatedAt, UpdatedAt không
                var createdAtProperty = entry.Entity.GetType().GetProperty("CreatedAt");
                var updatedAtProperty = entry.Entity.GetType().GetProperty("UpdatedAt");

                var properties = entry.Entity.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?));

                // Chuyển đổi các property kiểu DateTimeOffset sang UTC
                foreach (var prop in properties)
                {
                    var value = (DateTimeOffset?)prop.GetValue(entry.Entity);
                    if (value.HasValue)
                    {
                        prop.SetValue(entry.Entity, value.Value.ToUniversalTime());
                    }

                    if (prop.Name == "UpdatedAt" && updatedAtProperty != null && entry.State == EntityState.Modified)
                    {
                        entry.Property("UpdatedAt").CurrentValue = DateTimeOffset.UtcNow;
                        entry.Property("UpdatedAt").IsModified = true;  // Đánh dấu là đã thay đổi
                    }
                }
            }
        }
    }
}