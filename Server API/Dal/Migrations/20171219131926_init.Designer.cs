﻿// <auto-generated />
using DAL;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Dal.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20171219131926_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Entities.Comment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body")
                        .HasMaxLength(600);

                    b.Property<DateTime>("PublishDate");

                    b.HasKey("ID");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Entities.Country", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("FatherId")
                        .HasMaxLength(100);

                    b.Property<int>("Index");

                    b.Property<bool>("IsSpecial");

                    b.Property<string>("Value")
                        .HasMaxLength(100);

                    b.HasKey("ID");

                    b.ToTable("Countryes");
                });

            modelBuilder.Entity("Entities.ItemColor", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ColorHex")
                        .HasMaxLength(100);

                    b.Property<int?>("FatherId");

                    b.Property<string>("FatherName");

                    b.Property<int>("Index");

                    b.Property<bool>("IsSpecial");

                    b.Property<string>("Value")
                        .HasMaxLength(100);

                    b.HasKey("ID");

                    b.ToTable("ItemColors");
                });

            modelBuilder.Entity("Entities.RegistrationCaruselItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ImageName")
                        .HasMaxLength(100);

                    b.Property<int>("Index");

                    b.Property<string>("Text")
                        .HasMaxLength(800);

                    b.HasKey("ID");

                    b.ToTable("RegistrationCaruselItems");
                });

            modelBuilder.Entity("Entities.Role", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("FatherId");

                    b.Property<int>("Index");

                    b.Property<bool>("IsSpecial");

                    b.Property<string>("Value")
                        .HasMaxLength(100);

                    b.HasKey("ID");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Entities.Story", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body")
                        .HasMaxLength(1000);

                    b.Property<string>("Description")
                        .HasMaxLength(300);

                    b.Property<bool>("IsCommentsAlowed");

                    b.Property<bool>("IsDraft");

                    b.Property<DateTime>("PublishDate");

                    b.Property<string>("Title")
                        .HasMaxLength(200);

                    b.Property<int>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Storyes");
                });

            modelBuilder.Entity("Entities.StoryComment", b =>
                {
                    b.Property<int>("LeftId");

                    b.Property<int>("RightId");

                    b.HasKey("LeftId", "RightId");

                    b.HasIndex("RightId");

                    b.ToTable("StoryComments");
                });

            modelBuilder.Entity("Entities.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CountryId");

                    b.Property<DateTime>("CreationDate");

                    b.Property<int>("CultureId");

                    b.Property<string>("Email")
                        .HasMaxLength(70);

                    b.Property<int>("Gender");

                    b.Property<int?>("ItemColorID");

                    b.Property<DateTime>("LastLoginDate");

                    b.Property<string>("NickName")
                        .HasMaxLength(100);

                    b.Property<string>("Password")
                        .HasMaxLength(70);

                    b.Property<int>("YearOfBirth");

                    b.HasKey("ID");

                    b.HasIndex("CountryId");

                    b.HasIndex("ItemColorID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Entities.UserComment", b =>
                {
                    b.Property<int>("LeftId");

                    b.Property<int>("RightId");

                    b.HasKey("LeftId", "RightId");

                    b.HasIndex("RightId");

                    b.ToTable("UserComments");
                });

            modelBuilder.Entity("Entities.UserRole", b =>
                {
                    b.Property<int>("LeftId");

                    b.Property<int>("RightId");

                    b.HasKey("LeftId", "RightId");

                    b.HasIndex("RightId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Entities.Story", b =>
                {
                    b.HasOne("Entities.User", "User")
                        .WithMany("Storyes")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Entities.StoryComment", b =>
                {
                    b.HasOne("Entities.Story", "Story")
                        .WithMany("StoryComment")
                        .HasForeignKey("LeftId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Entities.Comment", "Comment")
                        .WithMany("StoryComment")
                        .HasForeignKey("RightId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Entities.User", b =>
                {
                    b.HasOne("Entities.Country", "Country")
                        .WithMany("Users")
                        .HasForeignKey("CountryId");

                    b.HasOne("Entities.ItemColor", "ItemColor")
                        .WithMany("Users")
                        .HasForeignKey("ItemColorID");
                });

            modelBuilder.Entity("Entities.UserComment", b =>
                {
                    b.HasOne("Entities.User", "User")
                        .WithMany("UserComment")
                        .HasForeignKey("LeftId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Entities.Comment", "Comment")
                        .WithMany("UserComment")
                        .HasForeignKey("RightId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Entities.UserRole", b =>
                {
                    b.HasOne("Entities.User", "User")
                        .WithMany("UserRole")
                        .HasForeignKey("LeftId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Entities.Role", "Role")
                        .WithMany("UserRole")
                        .HasForeignKey("RightId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
