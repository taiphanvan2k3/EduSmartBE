using AuthService.Commons;
using AuthService.Databases.Schemas;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Databases.InitDb
{
    public partial class DbInitializer
    {
        public async Task SeedDataDefault(DataContext context, UserManager<ApplicationUser> userManager)
        {
            if (!await context.Roles.AnyAsync())
            {
                var defaultRoles = new List<IdentityRole<int>>
                {
                    new() { Name = "Admin", NormalizedName = "ADMIN" },
                    new() { Name = "Teacher", NormalizedName = "TEACHER" },
                    new() { Name = "Student", NormalizedName = "STUDENT" }
                };

                await context.Roles.AddRangeAsync(defaultRoles);
                await context.SaveChangesAsync();
            }

            // Thêm dữ liệu vào bảng Users
            if (!await context.Users.AnyAsync())
            {
                await CreateDefaultAdminAccount(userManager);
            }
        }

        private static async Task CreateDefaultAdminAccount(UserManager<ApplicationUser> userManager)
        {
            bool isExistDefaultAccount = (await userManager.FindByEmailAsync(Constants.ADMIN_EMAIL)) != null;
            if (!isExistDefaultAccount)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "smartedu_admin",
                    Email = Constants.ADMIN_EMAIL,
                    FirstName = "Admin",
                    LastName = "User",
                    IsOnline = false,
                    IsActive = true,
                };

                var result = userManager.CreateAsync(adminUser, Constants.DEFAULT_ADMIN_PASSWORD).Result;
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    await userManager.ConfirmEmailAsync(adminUser, await userManager.GenerateEmailConfirmationTokenAsync(adminUser));
                }
            }
        }
    }
}