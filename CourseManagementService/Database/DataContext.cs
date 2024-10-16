using CourseManagementService.Database.Schemas;
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
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CourseTag> CourseTags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
    }
}