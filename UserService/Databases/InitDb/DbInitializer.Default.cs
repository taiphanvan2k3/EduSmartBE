using Microsoft.EntityFrameworkCore;
using TblRole = UserService.Databases.Schemas.Role;

namespace UserService.Databases.InitDb
{
    public partial class DbInitializer
    {
        public async Task SeedDataDefault()
        {
            if (!await context.Roles.AnyAsync())
            {
                var defaultRoles = new List<TblRole>
                {
                    new() { Id = 1, Name = "Admin" },
                    new() { Id = 2, Name = "Teacher" },
                    new() { Id = 3, Name = "Student" }
                };

                await context.Roles.AddRangeAsync(defaultRoles);
                await context.SaveChangesAsync();
            }
        }
    }
}