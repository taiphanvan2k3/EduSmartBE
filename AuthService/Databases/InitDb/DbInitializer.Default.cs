using AuthService.Databases.Schemas;

namespace AuthService.Databases.InitDb
{
    public partial class DbInitializer
    {
        private void SeedDataDefault(DataContext _context)
        {
            if (!_context.Roles.Any())
            {
                var defaultRoles = new List<Role>
                {
                    new() { Name = "Admin" },
                    new() { Name = "User" },
                    new() { Name = "Manager" }
                };
                _context.Roles.AddRange(defaultRoles);
                _context.SaveChanges();  // Lưu dữ liệu vào cơ sở dữ liệu
            }

            // Thêm dữ liệu vào bảng Screens
            if (!_context.Screens.Any())
            {
                var defaultScreens = new List<Screen>
                {
                    new() { Name = "Dashboard", Code = "DASH", Description = "Main dashboard" },
                    new() { Name = "Users", Code = "USER", Description = "User management screen" }
                };
                _context.Screens.AddRange(defaultScreens);
                _context.SaveChanges();  // Lưu dữ liệu vào cơ sở dữ liệu
            }

            // Lấy các Screen đã được tạo để sử dụng ScreenId cho Functions
            var dashboardScreen = _context.Screens.FirstOrDefault(s => s.Code == "DASH");
            var userScreen = _context.Screens.FirstOrDefault(s => s.Code == "USER");

            // Thêm dữ liệu vào bảng Functions sau khi đã có ScreenId
            if (!_context.Functions.Any())
            {
                var defaultFunctions = new List<Function>
                {
                    new() { ScreenId = dashboardScreen.Id, FunctionName = "View", Description = "View dashboard" },
                    new() { ScreenId = userScreen.Id, FunctionName = "Edit", Description = "Edit users" }
                };
                _context.Functions.AddRange(defaultFunctions);
                _context.SaveChanges();  // Lưu dữ liệu vào cơ sở dữ liệu
            }

            // Lấy các Function đã được tạo để sử dụng FunctionId cho Permissions
            var viewDashboardFunction = _context.Functions.FirstOrDefault(f => f.FunctionName == "View");
            var editUsersFunction = _context.Functions.FirstOrDefault(f => f.FunctionName == "Edit");

            // Thêm dữ liệu vào bảng Permissions
            if (!_context.Permissions.Any())
            {
                var defaultPermissions = new List<Permission>
                {
                    new() { RoleId = _context.Roles.First(r => r.Name == "Admin").Id, FunctionId = viewDashboardFunction.Id },
                    new() { RoleId = _context.Roles.First(r => r.Name == "Admin").Id, FunctionId = editUsersFunction.Id },
                    new() { RoleId = _context.Roles.First(r => r.Name == "User").Id, FunctionId = viewDashboardFunction.Id }
                };
                _context.Permissions.AddRange(defaultPermissions);
                _context.SaveChanges();  // Lưu dữ liệu vào cơ sở dữ liệu
            }

            // Thêm dữ liệu vào bảng Users
            if (!_context.Users.Any())
            {
                var defaultUsers = new List<User>
                {
                    new() { Username = "admin", Email = "admin@example.com", FirstName = "Admin", LastName = "User", Role = "Admin", IsOnline = false, IsActive = true },
                    new() { Username = "user", Email = "user@example.com", FirstName = "Regular", LastName = "User", Role = "User", IsOnline = false, IsActive = true }
                };
                _context.Users.AddRange(defaultUsers);
                _context.SaveChanges();  // Lưu dữ liệu vào cơ sở dữ liệu
            }

            // Lấy các User và Role đã được tạo để sử dụng UserId và RoleId cho UserRoles
            var adminUser = _context.Users.FirstOrDefault(u => u.Username == "admin");
            var regularUser = _context.Users.FirstOrDefault(u => u.Username == "user");

            // Thêm dữ liệu vào bảng UserRoles
            if (!_context.UserRoles.Any())
            {
                var defaultUserRoles = new List<UserRole>
                {
                    new() { UserId = adminUser.Id, RoleId = _context.Roles.First(r => r.Name == "Admin").Id },
                    new() { UserId = regularUser.Id, RoleId = _context.Roles.First(r => r.Name == "User").Id }
                };
                _context.UserRoles.AddRange(defaultUserRoles);
                _context.SaveChanges();  // Lưu dữ liệu vào cơ sở dữ liệu
            }

            _context.SaveChanges();
        }
    }
}