using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WPF_NhaMayCaoSu.Repository.Models;

namespace WPF_NhaMayCaoSu.Repository.Context
{
    public class CaoSuWpfDbContext : DbContext
    {
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<RFID> RFIDs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Camera> Cameras { get; set; }

        private string GetConnectionString()
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("dbsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var strConn = config.GetConnectionString("DefaultConnectionStringDB");
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
            // Sale
            modelBuilder.Entity<Sale>()
                .HasKey(s => s.SaleId);
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.RFID)
                .WithMany(r => r.Sales)
                .HasForeignKey(s => s.RFID_Id);
            modelBuilder.Entity<Sale>()
                .HasIndex(s => s.WeightImgageUrl)
                .IsUnique();
            modelBuilder.Entity<Sale>()
                .HasIndex(s => s.DensityImageUrl)
                .IsUnique();

            // Account
            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountId);
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RoleId);
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Username)
                .IsUnique();

            // RFID
            modelBuilder.Entity<RFID>()
                .HasKey(r => r.RFID_Id);
            modelBuilder.Entity<RFID>()
                .HasOne(r => r.Account)
                .WithMany(a => a.RFIDs)
                .HasForeignKey(r => r.AccountId);
            modelBuilder.Entity<RFID>()
                .HasIndex(r => r.RFIDCode)
                .IsUnique();

            // Role
            modelBuilder.Entity<Role>()
                .HasKey(r => r.RoleId);
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            // Camera
            modelBuilder.Entity<Camera>()
                .HasKey(c => c.Id);
            modelBuilder.Entity<Camera>()
                .HasIndex(c => c.IpCamera1)
                .IsUnique();
            modelBuilder.Entity<Camera>()
                .HasIndex(c => c.IpCamera2)
                .IsUnique();
        }
    }
}
