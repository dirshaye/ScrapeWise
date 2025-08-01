﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ScrapeWise.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250704103605_InitialPostgres")]
    partial class InitialPostgres
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Profile", b =>
                {
                    b.Property<int>("ProfileId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ProfileId"));

                    b.Property<string>("AvatarUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("DelayBetweenRequests")
                        .HasColumnType("integer");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserAgent")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("ProfileId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("ScrapingJob", b =>
                {
                    b.Property<int>("ScrapingJobId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ScrapingJobId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CssSelector")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TargetUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("ScrapingJobId");

                    b.HasIndex("UserId");

                    b.ToTable("ScrapingJobs");
                });

            modelBuilder.Entity("ScrapingJobTag", b =>
                {
                    b.Property<int>("ScrapingJobsScrapingJobId")
                        .HasColumnType("integer");

                    b.Property<int>("TagsTagId")
                        .HasColumnType("integer");

                    b.HasKey("ScrapingJobsScrapingJobId", "TagsTagId");

                    b.HasIndex("TagsTagId");

                    b.ToTable("ScrapingJobTags", (string)null);
                });

            modelBuilder.Entity("ScrapingResult", b =>
                {
                    b.Property<int>("ScrapingResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ScrapingResultId"));

                    b.Property<string>("ExtractedText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ScrapedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ScrapingJobId")
                        .HasColumnType("integer");

                    b.HasKey("ScrapingResultId");

                    b.HasIndex("ScrapingJobId");

                    b.ToTable("ScrapingResults");
                });

            modelBuilder.Entity("Tag", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("TagId"));

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("TagId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Profile", b =>
                {
                    b.HasOne("User", "User")
                        .WithOne("Profile")
                        .HasForeignKey("Profile", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ScrapingJob", b =>
                {
                    b.HasOne("User", "User")
                        .WithMany("ScrapingJobs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ScrapingJobTag", b =>
                {
                    b.HasOne("ScrapingJob", null)
                        .WithMany()
                        .HasForeignKey("ScrapingJobsScrapingJobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsTagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ScrapingResult", b =>
                {
                    b.HasOne("ScrapingJob", "ScrapingJob")
                        .WithMany("ScrapingResults")
                        .HasForeignKey("ScrapingJobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScrapingJob");
                });

            modelBuilder.Entity("ScrapingJob", b =>
                {
                    b.Navigation("ScrapingResults");
                });

            modelBuilder.Entity("User", b =>
                {
                    b.Navigation("Profile");

                    b.Navigation("ScrapingJobs");
                });
#pragma warning restore 612, 618
        }
    }
}
