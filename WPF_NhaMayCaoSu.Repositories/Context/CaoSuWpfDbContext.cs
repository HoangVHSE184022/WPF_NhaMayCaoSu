﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WPF_NhaMayCaoSu.Repository.Models;


namespace WPF_NhaMayCaoSu.Repository.Context
{
    public class CaoSuWpfDbContext : DbContext
    {
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Config> Cameras { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<RFID> RFIDs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Pricing> Pricings { get; set; }

        public CaoSuWpfDbContext(DbContextOptions<CaoSuWpfDbContext> options) : base(options)
        {

        }

        public CaoSuWpfDbContext()
        {

        }

        public string GetConnectionString()
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
            modelBuilder.Entity<Pricing>()
                .HasKey(p => p.PricingId);

            modelBuilder.Entity<Board>()
                .HasKey(b => b.BoardId);

            modelBuilder.Entity<Sale>()
                .HasKey(s => s.SaleId);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.RFID)
                .WithMany(r => r.Sales)
                .HasForeignKey(s => s.RFID_Id)
                .HasPrincipalKey(r => r.RFID_Id);

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

            modelBuilder.Entity<Config>()
                .HasKey(c => c.CameraId);

            modelBuilder.Entity<RFID>()
                .HasKey(r => r.RFID_Id);

            modelBuilder.Entity<Image>()
                .HasKey(i => i.ImageId);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Sale)
                .WithMany(s => s.Images)
                .HasForeignKey(i => i.SaleId);

            var adminRoleId = Guid.NewGuid();
            var userRoleId = Guid.NewGuid();

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    RoleId = adminRoleId,
                    RoleName = "Admin"
                },
                new Role
                {
                    RoleId = userRoleId,
                    RoleName = "User"
                }
            );

            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    AccountId = Guid.NewGuid(),
                    AccountName = "Administrator",
                    Username = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    CreatedDate = DateTime.UtcNow,
                    Status = 1,
                    RoleId = adminRoleId
                },
                new Account
                {
                    AccountId = Guid.NewGuid(),
                    AccountName = "Standard User",
                    Username = "user",
                    Password = BCrypt.Net.BCrypt.HashPassword("user123"),
                    CreatedDate = DateTime.UtcNow,
                    Status = 1,
                    RoleId = userRoleId
                }
            );

            modelBuilder.Entity<Config>().HasData(
                new Config
                {
                    CameraId = Guid.NewGuid(),
                    Camera1 = "N/A",
                    Camera2 = "N/A",
                    Time = 30,
                    GeneralPrice = 20000,
                    Status = 1
                }
            );
        }

    }
}
