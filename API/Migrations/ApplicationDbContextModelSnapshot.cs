﻿// <auto-generated />
using API.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("API.Models.Domain.Sample", b =>
                {
                    b.Property<int>("Mm")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Mm"));

                    b.Property<double>("Parameter1")
                        .HasColumnType("float");

                    b.Property<double>("Parameter2")
                        .HasColumnType("float");

                    b.Property<double>("Parameter3")
                        .HasColumnType("float");

                    b.Property<double>("Parameter4")
                        .HasColumnType("float");

                    b.HasKey("Mm");

                    b.ToTable("Samples");
                });
#pragma warning restore 612, 618
        }
    }
}
