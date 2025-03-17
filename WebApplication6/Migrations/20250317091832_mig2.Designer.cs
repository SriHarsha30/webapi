﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication6.Models;

#nullable disable

namespace WebApplication6.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20250317091832_mig2")]
    partial class mig2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebApplication6.Models.History", b =>
                {
                    b.Property<long>("Tenant_Phonenumber")
                        .HasColumnType("bigint");

                    b.Property<string>("Tenant_id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tenant_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("endTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("leased_property_id")
                        .HasColumnType("int");

                    b.Property<DateTime>("startTime")
                        .HasColumnType("datetime2");

                    b.ToTable("Histories");
                });

            modelBuilder.Entity("WebApplication6.Models.Lease", b =>
                {
                    b.Property<int>("LeaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LeaseId"));

                    b.Property<DateTime?>("EndDate")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool?>("Lease_status")
                        .IsRequired()
                        .HasColumnType("bit");

                    b.Property<bool?>("Owner_Signature")
                        .IsRequired()
                        .HasColumnType("bit");

                    b.Property<int?>("Property_Id")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<DateTime?>("StartDate")
                        .IsRequired()
                        .HasColumnType("datetime2");

                    b.Property<bool?>("Tenant_Signature")
                        .IsRequired()
                        .HasColumnType("bit");

                    b.HasKey("LeaseId");

                    b.HasIndex("ID");

                    b.HasIndex("Property_Id");

                    b.ToTable("Leases1", t =>
                        {
                            t.HasTrigger("InsertHistoryOnLeaseUpdate");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("WebApplication6.Models.Maintainance", b =>
                {
                    b.Property<int>("RequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RequestId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("ImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PropertyId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TenantId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RequestId");

                    b.ToTable("Maintainances");
                });

            modelBuilder.Entity("WebApplication6.Models.Notification", b =>
                {
                    b.Property<int>("Notification_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Notification_Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("notification_Descpirtion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("receiversId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("sendersId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Notification_Id");

                    b.ToTable("notifications1");
                });

            modelBuilder.Entity("WebApplication6.Models.Payment", b =>
                {
                    b.Property<int>("PaymentID")
                        .HasColumnType("int");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Ownerstatus")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PropertyId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Tenant_Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PaymentID");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("WebApplication6.Models.Property", b =>
                {
                    b.Property<int>("Property_Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Property_Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("AvailableStatus")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Owner_Id")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("PriceOfTheProperty")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Property_Id");

                    b.ToTable("Properties");
                });

            modelBuilder.Entity("WebApplication6.Models.Registration", b =>
                {
                    b.Property<string>("ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("D_O_B")
                        .HasColumnType("date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<long>("PhoneNumber")
                        .HasColumnType("bigint");

                    b.Property<string>("RoleofUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Signature")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Registrationss");
                });

            modelBuilder.Entity("WebApplication6.Models.Lease", b =>
                {
                    b.HasOne("WebApplication6.Models.Registration", "Tenant")
                        .WithMany()
                        .HasForeignKey("ID");

                    b.HasOne("WebApplication6.Models.Property", "Prop")
                        .WithMany()
                        .HasForeignKey("Property_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Prop");

                    b.Navigation("Tenant");
                });
#pragma warning restore 612, 618
        }
    }
}
