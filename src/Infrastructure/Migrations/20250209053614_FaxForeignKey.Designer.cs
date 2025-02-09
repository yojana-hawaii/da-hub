﻿// <auto-generated />
using System;
using Infrastructure.dbcontext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(DirectoryContext))]
    [Migration("20250209053614_FaxForeignKey")]
    partial class FaxForeignKey
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.directory.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("DepartmentName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentName")
                        .IsUnique()
                        .HasDatabaseName("ix_department_name");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Domain.directory.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AccountCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("EmployeeNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Extension")
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("HireDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("JobTitleId")
                        .HasColumnType("int");

                    b.Property<string>("Keyword")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("LocationId")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("NickName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhotoPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PrimaryManagerId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasAlternateKey("Email")
                        .HasName("ix_employee_email");

                    b.HasAlternateKey("Username")
                        .HasName("ix_employee_username");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("JobTitleId");

                    b.HasIndex("Keyword")
                        .HasDatabaseName("ix_employee_keyword");

                    b.HasIndex("LocationId");

                    b.HasIndex("PrimaryManagerId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Domain.directory.Fax", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<bool>("FaxForward")
                        .HasColumnType("bit");

                    b.Property<string>("FaxName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FaxNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("ForwardTo")
                        .HasColumnType("int");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("FaxNumber")
                        .IsUnique()
                        .HasDatabaseName("ix_fax_number");

                    b.HasIndex("LocationId");

                    b.ToTable("Faxes");
                });

            modelBuilder.Entity("Domain.directory.JobTitle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("JobTitleDescription")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("JobTitleName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("JobTitleName")
                        .IsUnique()
                        .HasDatabaseName("ix_jobTitle_name");

                    b.ToTable("JobTitles");
                });

            modelBuilder.Entity("Domain.directory.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LocationName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SubLocation")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("LocationName")
                        .IsUnique()
                        .HasDatabaseName("ix_location_name");

                    b.HasIndex("SubLocation")
                        .HasDatabaseName("ix_location_sublocation");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("Domain.directory.Employee", b =>
                {
                    b.HasOne("Domain.directory.Department", "Department")
                        .WithMany("Employees")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.directory.JobTitle", "JobTitle")
                        .WithMany("Employees")
                        .HasForeignKey("JobTitleId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.directory.Location", "Location")
                        .WithMany("Employees")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.directory.Employee", "PrimaryManager")
                        .WithMany("PrimaryStaff")
                        .HasForeignKey("PrimaryManagerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Department");

                    b.Navigation("JobTitle");

                    b.Navigation("Location");

                    b.Navigation("PrimaryManager");
                });

            modelBuilder.Entity("Domain.directory.Fax", b =>
                {
                    b.HasOne("Domain.directory.Department", "Department")
                        .WithMany("Faxes")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.directory.Location", "Location")
                        .WithMany("Faxes")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Department");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("Domain.directory.Department", b =>
                {
                    b.Navigation("Employees");

                    b.Navigation("Faxes");
                });

            modelBuilder.Entity("Domain.directory.Employee", b =>
                {
                    b.Navigation("PrimaryStaff");
                });

            modelBuilder.Entity("Domain.directory.JobTitle", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("Domain.directory.Location", b =>
                {
                    b.Navigation("Employees");

                    b.Navigation("Faxes");
                });
#pragma warning restore 612, 618
        }
    }
}
