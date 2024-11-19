using Microsoft.EntityFrameworkCore;
using PaymentService.Databases.Schemas;

namespace PaymentService.Databases
{
    public static class ModelCreate
    {
        public static ModelBuilder OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bank>(entity =>
            {
                entity.ToTable("Banks");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasMany(e => e.BankAccounts)
                    .WithOne(ba => ba.Bank)
                    .HasForeignKey(ba => ba.BankId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.ToTable("BankAccounts");
                entity.HasKey(e => e.Id);

                entity.HasMany(e => e.WithdrawalRequests)
                    .WithOne(wr => wr.BankAccount)
                    .HasForeignKey(wr => wr.BankAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.ToTable("PaymentTransactions");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OrderStatus)
                    .HasConversion<string>();
                entity.Property(e => e.PaymentMethod)
                    .HasConversion<string>();
                entity.Property(e => e.TransactionType)
                    .HasConversion<string>();
                entity.Property(e => e.RelatedInformation)
                    .HasColumnType("json");
            });

            modelBuilder.Entity<TeacherEarning>(entity =>
            {
                entity.ToTable("TeacherEarnings");
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.HasMany(e => e.WithdrawalRequests)
                    .WithOne(wr => wr.TeacherEarning)
                    .HasForeignKey(wr => wr.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<WithdrawalRequest>(entity =>
            {
                entity.ToTable("WithdrawalRequests");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            return modelBuilder;
        }

        public static void ConfigureForBaseEntity(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var createdAtProperty = entityType.ClrType.GetProperty("CreatedAt");
                var updatedAtProperty = entityType.ClrType.GetProperty("UpdatedAt");

                if (createdAtProperty != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(createdAtProperty.Name)
                        .HasDefaultValueSql("CURRENT_TIMESTAMP")
                        .ValueGeneratedOnAdd()
                        .IsRequired();
                }

                if (updatedAtProperty != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(updatedAtProperty.Name)
                        .HasDefaultValueSql("CURRENT_TIMESTAMP")
                        .ValueGeneratedOnAddOrUpdate()
                        .IsRequired();
                }
            }
        }
    }
}