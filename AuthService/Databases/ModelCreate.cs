using AuthService.Databases.Schemas;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Databases
{
    public class ModelCreate
    {
        public static ModelBuilder OnModelCreating(ModelBuilder modelBuilder)
        {
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasMany(e => e.UserRoles)
                    .WithOne(ur => ur.User)
                    .HasForeignKey(ur => ur.UserId);
                entity.HasMany(e => e.Logins)
                    .WithOne(l => l.User)
                    .HasForeignKey(l => l.UserId);
            });

            // Role configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.Id);
                entity.HasMany(e => e.UserRoles)
                    .WithOne(ur => ur.Role)
                    .HasForeignKey(ur => ur.RoleId);
                entity.HasMany(e => e.Permissions)
                    .WithOne(p => p.Role)
                    .HasForeignKey(p => p.RoleId);
            });

            // Permission configuration
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");
                entity.HasKey(p => new { p.RoleId, p.FunctionId });
                entity.HasOne(p => p.Function)
                    .WithMany(f => f.Permissions)
                    .HasForeignKey(p => p.FunctionId);
            });

            // Screen configuration
            modelBuilder.Entity<Screen>(entity =>
            {
                entity.ToTable("Screens");
                entity.HasKey(s => s.Id);
                entity.HasMany(s => s.Functions)
                    .WithOne(f => f.Screen)
                    .HasForeignKey(f => f.ScreenId);
            });

            // Function configuration
            modelBuilder.Entity<Function>(entity =>
            {
                entity.ToTable("Functions");
                entity.HasKey(f => f.Id);
            });

            // UserLogin configuration
            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.ToTable("UserLogins");
                entity.HasKey(ul => ul.Id);
            });

            // UserRole configuration
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
            });

            // CoursePermission configuration (if needed)
            modelBuilder.Entity<CoursePermission>(entity =>
            {
                entity.ToTable("CoursePermissions");
                entity.HasKey(cp => new { cp.CourseId, cp.AssistantId, cp.FunctionId });
            });

            return modelBuilder;
        }        
    }
}