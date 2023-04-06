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
    [Migration("20180206085231_0206a")]
    partial class _0206a
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Entities.BlockedUser", b =>
                {
                    b.Property<int>("LeftId");

                    b.Property<int>("RightId");

                    b.Property<int?>("OUserID");

                    b.HasKey("LeftId", "RightId");

                    b.HasIndex("OUserID");

                    b.ToTable("BlockedUsers");
                });

            modelBuilder.Entity("Entities.Comment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Body")
                        .HasMaxLength(600);

                    b.Property<string>("Language")
                        .HasMaxLength(6);

                    b.Property<DateTime>("PublishDate");

                    b.Property<int>("StoryId");

                    b.Property<int>("UserId");

                    b.HasKey("ID");

                    b.HasIndex("StoryId");

                    b.HasIndex("UserId");

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

            modelBuilder.Entity("Entities.HidenUser", b =>
                {
                    b.Property<int>("LeftId");

                    b.Property<int>("RightId");

                    b.Property<int?>("OUserID");

                    b.HasKey("LeftId", "RightId");

                    b.HasIndex("OUserID");

                    b.ToTable("HidenUsers");
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

            modelBuilder.Entity("Entities.Language", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("FatherId");

                    b.Property<int>("Index");

                    b.Property<bool>("IsSpecial");

                    b.Property<string>("Value")
                        .HasMaxLength(100);

                    b.HasKey("ID");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("Entities.LikedUserStory", b =>
                {
                    b.Property<int>("LeftId");

                    b.Property<int>("RightId");

                    b.HasKey("LeftId", "RightId");

                    b.HasIndex("RightId");

                    b.ToTable("LikedUserStories");
                });

            modelBuilder.Entity("Entities.MessageUser", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsRed");

                    b.Property<int>("LeftId");

                    b.Property<string>("Message")
                        .HasMaxLength(1000);

                    b.Property<DateTime>("MessageDate");

                    b.Property<int>("RightId");

                    b.HasKey("ID");

                    b.ToTable("MessageUsers");
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

                    b.Property<DateTime>("ApprovedDate");

                    b.Property<string>("Body")
                        .HasMaxLength(1000);

                    b.Property<string>("Description")
                        .HasMaxLength(300);

                    b.Property<bool>("IsCommentsAlowed");

                    b.Property<string>("Language")
                        .HasMaxLength(6);

                    b.Property<int>("Likes");

                    b.Property<DateTime>("PublishDate");

                    b.Property<int>("StoryStatus");

                    b.Property<string>("Title")
                        .HasMaxLength(200);

                    b.Property<int>("UserID");

                    b.Property<int>("Views");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Storyes");
                });

            modelBuilder.Entity("Entities.StoryTopic", b =>
                {
                    b.Property<int>("LeftId");

                    b.Property<int>("RightId");

                    b.HasKey("LeftId", "RightId");

                    b.HasIndex("RightId");

                    b.ToTable("StoryTopics");
                });

            modelBuilder.Entity("Entities.TailedUsers", b =>
                {
                    b.Property<int>("LeftId");

                    b.Property<int>("RightId");

                    b.Property<int?>("OUserID");

                    b.HasKey("LeftId", "RightId");

                    b.HasIndex("OUserID");

                    b.ToTable("TailedUsers");
                });

            modelBuilder.Entity("Entities.Topic", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("FatherId");

                    b.Property<int>("Index");

                    b.Property<bool>("IsSpecial");

                    b.Property<string>("Value")
                        .HasMaxLength(100);

                    b.HasKey("ID");

                    b.ToTable("Topics");
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

            modelBuilder.Entity("Entities.UserRole", b =>
                {
                    b.Property<int>("LeftId");

                    b.Property<int>("RightId");

                    b.HasKey("LeftId", "RightId");

                    b.HasIndex("RightId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Entities.ViewedUserStory", b =>
                {
                    b.Property<int>("LeftId");

                    b.Property<int>("RightId");

                    b.HasKey("LeftId", "RightId");

                    b.HasIndex("RightId");

                    b.ToTable("ViewedUserStories");
                });

            modelBuilder.Entity("Entities.BlockedUser", b =>
                {
                    b.HasOne("Entities.User", "IUser")
                        .WithMany("BlockedUser")
                        .HasForeignKey("LeftId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Entities.User", "OUser")
                        .WithMany()
                        .HasForeignKey("OUserID");
                });

            modelBuilder.Entity("Entities.Comment", b =>
                {
                    b.HasOne("Entities.Story", "Story")
                        .WithMany("Comments")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Entities.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Entities.HidenUser", b =>
                {
                    b.HasOne("Entities.User", "IUser")
                        .WithMany("HidenUser")
                        .HasForeignKey("LeftId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Entities.User", "OUser")
                        .WithMany()
                        .HasForeignKey("OUserID");
                });

            modelBuilder.Entity("Entities.LikedUserStory", b =>
                {
                    b.HasOne("Entities.User", "User")
                        .WithMany("LikedUserStory")
                        .HasForeignKey("LeftId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Entities.Story", "Story")
                        .WithMany("LikedUserStory")
                        .HasForeignKey("RightId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Entities.Story", b =>
                {
                    b.HasOne("Entities.User", "User")
                        .WithMany("Storyes")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Entities.StoryTopic", b =>
                {
                    b.HasOne("Entities.Story", "Story")
                        .WithMany("StoryTopic")
                        .HasForeignKey("LeftId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Entities.Topic", "Topic")
                        .WithMany("StoryTopic")
                        .HasForeignKey("RightId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Entities.TailedUsers", b =>
                {
                    b.HasOne("Entities.User", "IUser")
                        .WithMany("TailedUsers")
                        .HasForeignKey("LeftId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Entities.User", "OUser")
                        .WithMany()
                        .HasForeignKey("OUserID");
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

            modelBuilder.Entity("Entities.ViewedUserStory", b =>
                {
                    b.HasOne("Entities.User", "User")
                        .WithMany("ViewedUserStory")
                        .HasForeignKey("LeftId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Entities.Story", "Story")
                        .WithMany("ViewedUserStory")
                        .HasForeignKey("RightId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
