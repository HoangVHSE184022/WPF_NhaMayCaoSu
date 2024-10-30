﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WPF_NhaMayCaoSu.Repository.Context;

#nullable disable

namespace WPF_NhaMayCaoSu.Repository.Migrations
{
    [DbContext(typeof(CaoSuWpfDbContext))]
    partial class CaoSuWpfDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Account", b =>
                {
                    b.Property<Guid>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("Status")
                        .HasColumnType("bigint");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AccountId");

                    b.HasIndex("RoleId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Accounts");

                    b.HasData(
                        new
                        {
                            AccountId = new Guid("923d55ef-771b-4ec5-a3f6-412f86c4539e"),
                            AccountName = "Administrator",
                            CreatedDate = new DateTime(2024, 10, 30, 4, 50, 31, 38, DateTimeKind.Utc).AddTicks(3854),
                            Password = "$2a$11$2FVgXxRJ87rxK0S2tMDjoeQ84nUSz59Y5Dukn3QbowlMQTGHaq/w.",
                            RoleId = new Guid("9cc6ba19-dc33-4ed9-8cf7-6c49b16b278a"),
                            Status = 1L,
                            Username = "admin"
                        },
                        new
                        {
                            AccountId = new Guid("3d83bb1e-7a34-4202-bee3-7edb3662f246"),
                            AccountName = "Standard User",
                            CreatedDate = new DateTime(2024, 10, 30, 4, 50, 31, 152, DateTimeKind.Utc).AddTicks(203),
                            Password = "$2a$11$Tabyld/QsrjddILbnYgSruNuqpL/k0UaEDQY.X8LxZGBHte0jIO9u",
                            RoleId = new Guid("d932fcdd-16e3-403a-b8ee-cdba0a04cb57"),
                            Status = 1L,
                            Username = "user"
                        });
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Board", b =>
                {
                    b.Property<Guid>("BoardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BoardIp")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BoardMacAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BoardMode")
                        .HasColumnType("int");

                    b.Property<string>("BoardName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BoardStatus")
                        .HasColumnType("int");

                    b.HasKey("BoardId");

                    b.ToTable("Boards");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Config", b =>
                {
                    b.Property<Guid>("CameraId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Camera1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Camera2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("GeneralPrice")
                        .HasColumnType("real");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.Property<int>("Time")
                        .HasColumnType("int");

                    b.HasKey("CameraId");

                    b.ToTable("Cameras");

                    b.HasData(
                        new
                        {
                            CameraId = new Guid("282c4ded-e8e5-468f-9d07-546cafb63542"),
                            Camera1 = "N/A",
                            Camera2 = "N/A",
                            GeneralPrice = 20000f,
                            Status = (short)1,
                            Time = 30
                        });
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Customer", b =>
                {
                    b.Property<Guid>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.Property<float?>("bonusPrice")
                        .HasColumnType("real");

                    b.HasKey("CustomerId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Image", b =>
                {
                    b.Property<Guid>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("ImageType")
                        .HasColumnType("smallint");

                    b.Property<Guid>("SaleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ImageId");

                    b.HasIndex("SaleId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Pricing", b =>
                {
                    b.Property<Guid>("PricingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<float?>("Price")
                        .HasColumnType("real");

                    b.HasKey("PricingId");

                    b.ToTable("Pricings");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.RFID", b =>
                {
                    b.Property<Guid>("RFID_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RFIDCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.HasKey("RFID_Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("RFIDs");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Role", b =>
                {
                    b.Property<Guid>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("RoleId");

                    b.HasIndex("RoleName")
                        .IsUnique();

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            RoleId = new Guid("9cc6ba19-dc33-4ed9-8cf7-6c49b16b278a"),
                            RoleName = "Admin"
                        },
                        new
                        {
                            RoleId = new Guid("d932fcdd-16e3-403a-b8ee-cdba0a04cb57"),
                            RoleName = "User"
                        });
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Sale", b =>
                {
                    b.Property<Guid>("SaleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<float?>("BonusPrice")
                        .HasColumnType("real");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastEditedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float?>("ProductDensity")
                        .HasColumnType("real");

                    b.Property<float?>("ProductWeight")
                        .HasColumnType("real");

                    b.Property<string>("RFIDCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RFID_Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<float?>("SalePrice")
                        .HasColumnType("real");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.Property<float?>("TareWeight")
                        .HasColumnType("real");

                    b.Property<float?>("TotalPrice")
                        .HasColumnType("real");

                    b.HasKey("SaleId");

                    b.HasIndex("RFID_Id");

                    b.ToTable("Sales");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Account", b =>
                {
                    b.HasOne("WPF_NhaMayCaoSu.Repository.Models.Role", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Image", b =>
                {
                    b.HasOne("WPF_NhaMayCaoSu.Repository.Models.Sale", "Sale")
                        .WithMany("Images")
                        .HasForeignKey("SaleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sale");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.RFID", b =>
                {
                    b.HasOne("WPF_NhaMayCaoSu.Repository.Models.Customer", "Customer")
                        .WithMany("RFIDs")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Sale", b =>
                {
                    b.HasOne("WPF_NhaMayCaoSu.Repository.Models.RFID", "RFID")
                        .WithMany("Sales")
                        .HasForeignKey("RFID_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RFID");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Customer", b =>
                {
                    b.Navigation("RFIDs");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.RFID", b =>
                {
                    b.Navigation("Sales");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Role", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Sale", b =>
                {
                    b.Navigation("Images");
                });
#pragma warning restore 612, 618
        }
    }
}
