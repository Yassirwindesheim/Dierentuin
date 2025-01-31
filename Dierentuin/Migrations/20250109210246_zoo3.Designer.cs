﻿// <auto-generated />
using System;
using Dierentuin.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Dierentuin.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("20250109210246_zoo3")]
    partial class zoo3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Dierentuin.Models.Animal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ActivityPattern")
                        .HasColumnType("int");

                    b.Property<int?>("AnimalId")
                        .HasColumnType("int");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("Diet")
                        .HasColumnType("int");

                    b.Property<int?>("EnclosureId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SecurityRequirement")
                        .HasColumnType("int");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.Property<double>("SpaceRequirement")
                        .HasColumnType("float");

                    b.Property<string>("Species")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ZooId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AnimalId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("EnclosureId");

                    b.HasIndex("ZooId");

                    b.ToTable("Animals");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ActivityPattern = 2,
                            CategoryId = 1,
                            Diet = 0,
                            EnclosureId = 1,
                            Name = "Lion",
                            SecurityRequirement = 2,
                            Size = 4,
                            SpaceRequirement = 200.0,
                            Species = "Panthera leo"
                        },
                        new
                        {
                            Id = 2,
                            ActivityPattern = 0,
                            CategoryId = 2,
                            Diet = 0,
                            EnclosureId = 2,
                            Name = "Eagle",
                            SecurityRequirement = 1,
                            Size = 3,
                            SpaceRequirement = 100.0,
                            Species = "Aquila chrysaetos"
                        },
                        new
                        {
                            Id = 3,
                            ActivityPattern = 1,
                            CategoryId = 3,
                            Diet = 0,
                            EnclosureId = 3,
                            Name = "Python",
                            SecurityRequirement = 2,
                            Size = 4,
                            SpaceRequirement = 50.0,
                            Species = "Python reticulatus"
                        });
                });

            modelBuilder.Entity("Dierentuin.Models.Category", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Mammals"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Birds"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Reptiles"
                        });
                });

            modelBuilder.Entity("Dierentuin.Models.Enclosure", b =>
                {
                    b.Property<int?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int?>("Id"));

                    b.Property<int>("Climate")
                        .HasColumnType("int");

                    b.Property<int>("HabitatType")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SecurityLevel")
                        .HasColumnType("int");

                    b.Property<double>("Size")
                        .HasColumnType("float");

                    b.Property<int?>("ZooId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ZooId");

                    b.ToTable("Enclosures");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Climate = 0,
                            HabitatType = 8,
                            Name = "Savanna Exhibit",
                            SecurityLevel = 1,
                            Size = 1000.0
                        },
                        new
                        {
                            Id = 2,
                            Climate = 1,
                            HabitatType = 1,
                            Name = "Aviary",
                            SecurityLevel = 0,
                            Size = 500.0
                        },
                        new
                        {
                            Id = 3,
                            Climate = 2,
                            HabitatType = 2,
                            Name = "Reptile House",
                            SecurityLevel = 2,
                            Size = 300.0
                        });
                });

            modelBuilder.Entity("Dierentuin.Models.Zoo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AnimalIds")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EnclosureIds")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Zoos");
                });

            modelBuilder.Entity("Dierentuin.Models.Animal", b =>
                {
                    b.HasOne("Dierentuin.Models.Animal", null)
                        .WithMany("Prey")
                        .HasForeignKey("AnimalId");

                    b.HasOne("Dierentuin.Models.Category", "Category")
                        .WithMany("Animals")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Dierentuin.Models.Enclosure", "Enclosure")
                        .WithMany("Animals")
                        .HasForeignKey("EnclosureId");

                    b.HasOne("Dierentuin.Models.Zoo", null)
                        .WithMany("Animals")
                        .HasForeignKey("ZooId");

                    b.Navigation("Category");

                    b.Navigation("Enclosure");
                });

            modelBuilder.Entity("Dierentuin.Models.Enclosure", b =>
                {
                    b.HasOne("Dierentuin.Models.Zoo", null)
                        .WithMany("Enclosures")
                        .HasForeignKey("ZooId");
                });

            modelBuilder.Entity("Dierentuin.Models.Animal", b =>
                {
                    b.Navigation("Prey");
                });

            modelBuilder.Entity("Dierentuin.Models.Category", b =>
                {
                    b.Navigation("Animals");
                });

            modelBuilder.Entity("Dierentuin.Models.Enclosure", b =>
                {
                    b.Navigation("Animals");
                });

            modelBuilder.Entity("Dierentuin.Models.Zoo", b =>
                {
                    b.Navigation("Animals");

                    b.Navigation("Enclosures");
                });
#pragma warning restore 612, 618
        }
    }
}
