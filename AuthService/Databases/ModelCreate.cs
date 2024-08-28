using AuthService.Databases.Schemas;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Databases
{
    public static class ModelCreate
    {
        public static ModelBuilder OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");
                entity.HasKey(p => new { p.RoleId, p.FunctionId });
                entity.HasOne(p => p.Function)
                    .WithMany(f => f.Permissions)
                    .HasForeignKey(p => p.FunctionId);
            });

            modelBuilder.Entity<Screen>(entity =>
            {
                entity.ToTable("Screens");
                entity.HasKey(s => s.Id);
                entity.HasMany(s => s.Functions)
                    .WithOne(f => f.Screen)
                    .HasForeignKey(f => f.ScreenId);
            });

            modelBuilder.Entity<Function>(entity =>
            {
                entity.ToTable("Functions");
                entity.HasKey(f => f.Id);
            });

            modelBuilder.Entity<CoursePermission>(entity =>
            {
                entity.ToTable("CoursePermissions");
                entity.HasKey(cp => new { cp.CourseId, cp.AssistantId, cp.FunctionId });
            });

            return modelBuilder;
        }
    }
}