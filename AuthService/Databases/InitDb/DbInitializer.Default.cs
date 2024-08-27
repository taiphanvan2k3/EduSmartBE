using AuthService.Databases.Schemas;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Databases.InitDb
{
    public partial class DbInitializer
    {
        public void SeedDataDefault(DataContext context)
        {
            if (!context.Roles.Any())
            {
                var defaultRoles = new List<IdentityRole<int>>
                {
                    new() { Name = "Admin", NormalizedName = "ADMIN" },
                    new() { Name = "Teacher", NormalizedName = "TEACHER" },
                    new() { Name = "Student", NormalizedName = "STUDENT" }
                };

                context.Roles.AddRange(defaultRoles);
                context.SaveChanges();
            }

            // Thêm dữ liệu vào bảng Users
            if (!context.Users.Any())
            {
                var defaultUsers = new List<User>
                {
                    new()
                    {
                        UserName = "admin",
                        Email = "admin@example.com",
                        FirstName = "Admin",
                        LastName = "User",
                        IsOnline = false,
                        IsActive = true
                    },
                };
                context.Users.AddRange(defaultUsers);
                context.SaveChanges();
            }

            // Thêm dữ liệu vào bảng UserRoles
            var admin = context.Users.FirstOrDefault(u => u.UserName == "admin");
            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
            if (!context.UserRoles.Any() && admin != null && adminRole != null)
            {
                var defaultUserRoles = new List<IdentityUserRole<int>>
                {
                    new()
                    {
                        UserId = admin.Id,
                        RoleId = adminRole.Id
                    },
                };
                context.UserRoles.AddRange(defaultUserRoles);
                context.SaveChanges();
            }
        }
    }
}