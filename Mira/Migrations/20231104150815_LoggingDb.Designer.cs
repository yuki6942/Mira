﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Mira.Database;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mira.Migrations
{
    [DbContext(typeof(MiraContext))]
    [Migration("20231104150815_LoggingDb")]
    partial class LoggingDb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Mira.Database.Entities.Guild", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("PremiumStatus")
                        .HasColumnType("boolean");

                    b.HasKey("GuildId");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("Mira.Database.Entities.Logging", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("LoggingChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.HasKey("GuildId");

                    b.ToTable("Loggings");
                });

            modelBuilder.Entity("Mira.Database.Entities.StardewCharacter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Birthday")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Dislikes")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Hates")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Likes")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Loves")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Neutral")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Villager")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("StardewCharacters");
                });

            modelBuilder.Entity("Mira.Database.Entities.User", b =>
                {
                    b.Property<decimal>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("PremiumStatus")
                        .HasColumnType("boolean");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
