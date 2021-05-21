using eBanking.BusinessModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eBanking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<BankAccount> BankAccounts { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<CurrencyRateHistory> CurrencyRateHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankAccount>()
                .HasOne(t => t.ApplicationUser)
                .WithMany(s => s.Accounts)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BankAccount>()
                .HasOne(t => t.Currency)
                .WithMany(s => s.Accounts)
                .HasForeignKey(t => t.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.AccountFrom)
                .WithMany(s => s.TransactionsFrom)
                .HasForeignKey(t => t.AccountFromId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.AccountTo)
                .WithMany(s => s.TransactionsTo)
                .HasForeignKey(t => t.AccountToId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CurrencyRateHistory>()
                .HasOne(t => t.Currency)
                .WithMany(s => s.History)
                .HasForeignKey(t => t.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Currency>().HasData(new Currency { Id = 1, Name = "EUR", Rate = 1 });

            base.OnModelCreating(modelBuilder);

        }
    }
}