﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShippingContainer;

namespace ShippingContainer.Migrations
{
    [DbContext(typeof(ShippingRepository))]
    [Migration("20180814102530_AddContainerModels")]
    partial class AddContainerModels
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ShippingContainer.Models.Container", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ContainerId");

                    b.Property<double>("ProductCount");

                    b.Property<int>("TripId");

                    b.HasKey("Id");

                    b.ToTable("Containers");
                });

            modelBuilder.Entity("ShippingContainer.Models.TemperatureRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ContainerId");

                    b.Property<DateTime>("Time");

                    b.Property<int>("TripId");

                    b.Property<float>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ContainerId");

                    b.ToTable("TemperatureRecords");
                });

            modelBuilder.Entity("ShippingContainer.Models.Trip", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Created");

                    b.Property<string>("Name");

                    b.Property<double>("SpoilDuration");

                    b.Property<float>("SpoilTemperature");

                    b.Property<DateTime>("Updated");

                    b.HasKey("Id");

                    b.ToTable("Trips");
                });

            modelBuilder.Entity("ShippingContainer.Models.TemperatureRecord", b =>
                {
                    b.HasOne("ShippingContainer.Models.Container")
                        .WithMany("Temperatures")
                        .HasForeignKey("ContainerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
