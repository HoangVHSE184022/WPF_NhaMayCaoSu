using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Context
{
    public class CaoSuWpfDbContext : DbContext
    {
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<RFID> RFIDs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Customer> Customers { get; set; }

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
                .HasOne(s => s.RFID)
                .WithOne()
                .HasForeignKey<Sale>(s => s.RFIDCode)
                .HasPrincipalKey<RFID>(r => r.RFIDCode);

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

            modelBuilder.Entity<Role>()
                .HasKey(r => r.RoleId);

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<Camera>()
                .HasKey(c => c.CameraId);

            modelBuilder.Entity<RFID>()
                .HasKey(r => r.RFID_Id);

            modelBuilder.Entity<RFID>()
                .HasIndex(r => r.RFIDCode)
                .IsUnique();

            modelBuilder.Entity<Image>()
                .HasKey(i => i.ImageId);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Sale)
                .WithMany(s => s.Images)
                .HasForeignKey(i => i.SaleId);
        }

    }
}
