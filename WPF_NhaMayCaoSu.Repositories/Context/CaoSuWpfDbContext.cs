using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Context
{
    public class CaoSuWpfDbContext : DbContext
    {
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Camera> Cameras { get; set; }

        private string GetConnectionString()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("dbsettings.json", optional: true, reloadOnChange: true)
                .Build();

            string strConn = config.GetConnectionString("DefaultConnectionStringDB");
            if (string.IsNullOrEmpty(strConn))
            {
                throw new Exception("Connection string not found!");
            }

            return strConn;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>()
                .HasKey(s => s.SaleId);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.RFIDCode)
                .HasPrincipalKey(c => c.RFIDCode);

            modelBuilder.Entity<Sale>()
                .HasIndex(s => s.DensityImageUrl)
                .IsUnique();

            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountId);
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RoleId);
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasKey(c => c.CustomerId);
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.RFIDCode)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasKey(r => r.RoleId);
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<Camera>()
                .HasKey(c => c.CameraId);
        }
    }
}
