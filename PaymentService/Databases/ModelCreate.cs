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
                    .HasForeignKey(ba => ba.BankId);
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.ToTable("BankAccounts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasMany(e => e.WithdrawalRequests)
                    .WithOne(wr => wr.BankAccount)
                    .HasForeignKey(wr => wr.BankAccountId);
            });
            
            modelBuilder.Entity<StudentTransaction>(entity =>
            {
                entity.ToTable("StudentTransactions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<TeacherEarning>(entity =>
            {
                entity.ToTable("TeacherEarnings");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).ValueGeneratedNever();
            });

            modelBuilder.Entity<WithdrawalRequest>(entity =>
            {
                entity.ToTable("WithdrawalRequests");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            return modelBuilder;
        }
    }
}