using UserService.Databases.Schemas;
using Microsoft.EntityFrameworkCore;

namespace UserService.Databases
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

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.ToTable("UserInfos");
                entity.HasKey(e => e.UserId);
            });

            modelBuilder.Entity<UserInfo>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<UserInfo>(ui => ui.UserId);

            return modelBuilder;
        }
    }
}