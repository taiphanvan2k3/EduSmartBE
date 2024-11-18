using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using PaymentService.Databases.Schemas;

namespace PaymentService.Databases
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
            ModelCreate.ConfigureForBaseEntity(builder);
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

        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<TeacherEarning> TeacherEarnings { get; set; }
        public DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }

        public override int SaveChanges()
        {
            SetDateTimeOffsetsToUtc();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetDateTimeOffsetsToUtc();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Because PostgreSQL does not allow DateTimeOffsets with non-UTC time zones, we need to convert all DateTimeOffsets 
        /// to UTC before saving them to the database.
        /// </summary>
        private void SetDateTimeOffsetsToUtc()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                var properties = entry.Entity.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?));

                foreach (var prop in properties)
                {
                    var value = (DateTimeOffset?)prop.GetValue(entry.Entity);
                    if (value.HasValue)
                    {
                        prop.SetValue(entry.Entity, value.Value.ToUniversalTime());
                    }
                }
            }
        }
    }
}