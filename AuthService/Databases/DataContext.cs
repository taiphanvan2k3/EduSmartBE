using System.Data;
using System.Data.Common;
using AuthService.Databases.Schemas;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Databases
{
    public class DataContext : DbContext
    {

        private readonly IHttpContextAccessor? _context;

        // Constructor không có IHttpContextAccessor cho design-time
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // Constructor dùng cho runtime với IHttpContextAccessor
        public DataContext(DbContextOptions<DataContext> options, IHttpContextAccessor? context) : base(options) 
        {
            _context = context;
        }

        public DbConnection GetConnection()
        {
            DbConnection _connection = Database.GetDbConnection();
            _connection.Close();
            if(_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            return _connection;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<CoursePermission> CoursePermissions { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ModelCreate.OnModelCreating(builder);
        }
    }
}