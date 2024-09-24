using System.Data;
using System.Data.Common;
using UserService.Databases.Schemas;
using Microsoft.EntityFrameworkCore;

namespace UserService.Databases
{
    public class DataContext : DbContext
    {
        private readonly IHttpContextAccessor _context;

        // Constructor không có IHttpContextAccessor cho design-time
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // Constructor dùng cho runtime với IHttpContextAccessor
        public DataContext(DbContextOptions<DataContext> options, IHttpContextAccessor context) : base(options)
        {
            _context = context;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ModelCreate.OnModelCreating(builder);
        }

        public DbConnection GetConnection()
        {
            DbConnection _connection = Database.GetDbConnection();
            _connection.Close();
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
            return _connection;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }

        public override int SaveChanges()
        {
            SetDateTimeToUtc();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetDateTimeToUtc();
            return base.SaveChangesAsync(cancellationToken);
        }

        // Hàm để chuyển đổi DateTime thành UTC cho User entity
        private void SetDateTimeToUtc()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.Entity is User && (e.State == EntityState.Added || e.State == EntityState.Modified)))
            {
                var entity = (User)entry.Entity;

                if (entity.LastLogin.Kind == DateTimeKind.Unspecified)
                {
                    entity.LastLogin = DateTime.SpecifyKind(entity.LastLogin, DateTimeKind.Utc);
                }
                else
                {
                    entity.LastLogin = entity.LastLogin.ToUniversalTime();
                }

                if (entity.LastLogout.Kind == DateTimeKind.Unspecified)
                {
                    entity.LastLogout = DateTime.SpecifyKind(entity.LastLogout, DateTimeKind.Utc);
                }
                else
                {
                    entity.LastLogout = entity.LastLogout.ToUniversalTime();
                }
            }
        }
    }
}