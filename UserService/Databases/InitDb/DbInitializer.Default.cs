using Microsoft.EntityFrameworkCore;
using UserService.Databases.Schemas;

namespace UserService.Databases.InitDb
{
    public partial class DbInitializer
    {
        public async Task SeedDataDefault(DataContext context)
        {
            if (!await context.Users.AnyAsync())
            {
                var defaultUsers = new List<User>
                {
                    new User
                    {
                        Id = "admin",
                        UserName = "admin",
                        LastLogin = DateTime.UtcNow,
                        LastLogout = DateTime.UtcNow,
                        IsOnline = false,
                        IsActive = true
                    },
                    new User
                    {
                        Id = "teacher1",
                        UserName = "teacher1",
                        LastLogin = DateTime.UtcNow,
                        LastLogout = DateTime.UtcNow,
                        IsOnline = false,
                        IsActive = true
                    },
                    new User
                    {
                        Id = "student1",
                        UserName = "student1",
                        LastLogin = DateTime.UtcNow,
                        LastLogout = DateTime.UtcNow,
                        IsOnline = false,
                        IsActive = true
                    }
                };

                await context.Users.AddRangeAsync(defaultUsers);
                await context.SaveChangesAsync();

                var defaultUserInfos = new List<UserInfo>
                {
                    new UserInfo
                    {
                        UserId = "admin",
                        FirstName = "Admin",
                        LastName = "User",
                        AvatarURL = "https://example.com/avatars/admin.png",
                        Phone = "123-456-7890",
                        Gender = 0 // Giới tính: 0 = Không xác định, 1 = Nam, 2 = Nữ
                    },
                    new UserInfo
                    {
                        UserId = "teacher1",
                        FirstName = "John",
                        LastName = "Doe",
                        AvatarURL = "https://example.com/avatars/teacher1.png",
                        Phone = "098-765-4321",
                        Gender = 1
                    },
                    new UserInfo
                    {
                        UserId = "student1",
                        FirstName = "Jane",
                        LastName = "Smith",
                        AvatarURL = "https://example.com/avatars/student1.png",
                        Phone = "555-555-5555",
                        Gender = 2
                    }
                };

                await context.UserInfos.AddRangeAsync(defaultUserInfos);
                await context.SaveChangesAsync();
            }
        }
    }
}