using System.Data;
using System.Data.Common;
using AuthService.Databases.Schemas;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Databases
{
    // Mặc định User của Identity sử dụng key kiểu string, ta cần custom lại để sử dụng key kiểu int
    public class DataContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ModelCreate.OnModelCreating(builder);
            ModelCreate.ConfigureForBaseEntity(builder);

            // Đổi tên bảng mặc định của Identity, thay vì có tiền tố AspNet
            builder.Entity<IdentityRole<int>>().ToTable("Roles");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<CoursePermission> CoursePermissions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}