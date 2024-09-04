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

                    b.HasIndex("RFIDCode")
                        .IsUnique();

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
                        .WithOne()
                        .HasForeignKey("WPF_NhaMayCaoSu.Repository.Models.Sale", "RFIDCode")
                        .HasPrincipalKey("WPF_NhaMayCaoSu.Repository.Models.RFID", "RFIDCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RFID");
                });

            modelBuilder.Entity("WPF_NhaMayCaoSu.Repository.Models.Customer", b =>
                {
                    b.Navigation("RFIDs");
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
