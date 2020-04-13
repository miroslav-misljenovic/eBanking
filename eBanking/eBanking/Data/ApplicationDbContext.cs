using eBanking.BusinessModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eBanking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Currency> Currencies { get; set; }

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

            modelBuilder.Entity<Currency>().HasData(new Currency { Id = 1, Name = "RSD", Rate = 1 });
            modelBuilder.Entity<Currency>().HasData(new Currency { Id = 2, Name = "EUR", Rate = 117.5 });
            modelBuilder.Entity<Currency>().HasData(new Currency { Id = 3, Name = "USD", Rate = 107.4 });
            modelBuilder.Entity<Currency>().HasData(new Currency { Id = 4, Name = "RUB", Rate = 1.46 });
            modelBuilder.Entity<Currency>().HasData(new Currency { Id = 5, Name = "GBP", Rate = 134.7 });

            base.OnModelCreating(modelBuilder);

        }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }
    }
}
