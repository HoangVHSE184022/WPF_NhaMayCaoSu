﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WPF_NhaMayCaoSu.Repository.Context;

#nullable disable

namespace WPF_NhaMayCaoSu.Repository.Migrations
{
    [DbContext(typeof(CaoSuWpfDbContext))]
    [Migration("20240910103932_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
                            AccountId = new Guid("aa832d40-544b-47c7-9e8e-45d978c6397a"),
                            AccountName = "Administrator",
                            CreatedDate = new DateTime(2024, 9, 10, 10, 39, 32, 49, DateTimeKind.Utc).AddTicks(3494),
                            Password = "$2a$11$eNGmqLRPFqBVOoTWSlLi2efaifsVTltzJGnW9eh0K17IS11n0BVDK",
                            RoleId = new Guid("26d39ce0-5098-43a3-9b60-cb68f2bf7468"),
                            Status = 1L,
                            Username = "admin"
                        },
                        new
                        {
                            AccountId = new Guid("5086d0a7-69a4-4ee9-b8d8-9ed6a5fd74f4"),
                            AccountName = "Standard User",
                            CreatedDate = new DateTime(2024, 9, 10, 10, 39, 32, 246, DateTimeKind.Utc).AddTicks(8773),
                            Password = "$2a$11$bzeOSKX/V8V0FlRNaGy4BO7ZB3vbwsxZyOcJbOf4n9L7kRDaD7Z1y",
                            RoleId = new Guid("36a16001-a54c-4242-8100-2b6c6c602875"),
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

                    b.Property<string>("BoardMode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BoardName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BoardId");

                    b.ToTable("Boards");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Camera", b =>
                {
                    b.Property<Guid>("CameraId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Camera1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Camera2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.HasKey("CameraId");

                    b.ToTable("Cameras");
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
                        .HasColumnType("nvarchar(450)");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.HasKey("RFID_Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("RFIDCode")
                        .IsUnique();

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
                            RoleId = new Guid("26d39ce0-5098-43a3-9b60-cb68f2bf7468"),
                            RoleName = "Admin"
                        },
                        new
                        {
                            RoleId = new Guid("36a16001-a54c-4242-8100-2b6c6c602875"),
                            RoleName = "User"
                        });
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Sale", b =>
                {
                    b.Property<Guid>("SaleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastEditedTime")
                        .HasColumnType("datetime2");

                    b.Property<float?>("ProductDensity")
                        .HasColumnType("real");

                    b.Property<float?>("ProductWeight")
                        .HasColumnType("real");

                    b.Property<string>("RFIDCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.HasKey("SaleId");

                    b.HasIndex("RFIDCode");

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
                        .HasForeignKey("RFIDCode")
                        .HasPrincipalKey("RFIDCode")
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