using AuthService.Databases.Schemas;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Databases
{
    public static class ModelCreate
    {
        public static ModelBuilder OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Screen>(entity =>
            {
                entity.ToTable("Screens");
                entity.HasKey(s => s.Id);
                entity.HasMany(s => s.Functions)
                    .WithOne(f => f.Screen)
                    .HasForeignKey(f => f.ScreenId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Function>(entity =>
            {
                entity.ToTable("Functions");
                entity.HasKey(f => f.Id);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");
                entity.HasKey(p => new { p.RoleId, p.FunctionId });
                entity.HasOne(p => p.Function)
                    .WithMany(f => f.Permissions)
                    .HasForeignKey(p => p.FunctionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CoursePermission>(entity =>
            {
                entity.ToTable("CoursePermissions");
                entity.HasKey(cp => new { cp.CourseId, cp.AssistantId, cp.FunctionId });
                entity.HasOne(cp => cp.Function)
                    .WithMany(f => f.CoursePermissions)
                    .HasForeignKey(cp => cp.FunctionId)
                    .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa Function nếu có CoursePermission
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");
                entity.HasKey(rt => rt.Id);
                entity.HasIndex(rt => new { rt.Token, rt.IsRevoked });
                entity.HasOne(rt => rt.User)
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(rt => rt.IsRevoked).HasDefaultValue(false);
            });

            return modelBuilder;
        }
    }
}