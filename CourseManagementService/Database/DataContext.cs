using System.Data;
using System.Data.Common;
using CourseManagementService.Database.Schemas;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseManagementService.Database
{
    // Mặc định User của Identity sử dụng key kiểu string, ta cần custom lại để sử dụng key kiểu int
    public class DataContext : DbContext
    {
        private readonly IHttpContextAccessor _context;

        // Constructor không có IHttpContextAccessor cho design-time
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // Constructor dùng cho runtime với IHttpContextAccessor
        public DataContext(DbContextOptions<DataContext> options, IHttpContextAccessor context) : base(options)
        {
            _context = context;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ModelCreate.OnModelCreating(builder);
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CourseTag> CourseTags { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}