using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dal.Migrations
{
    public partial class _0107 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "Storyes");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "Storyes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StoryStatus",
                table: "Storyes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "Storyes");

            migrationBuilder.DropColumn(
                name: "StoryStatus",
                table: "Storyes");

            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "Storyes",
                nullable: false,
                defaultValue: false);
        }
    }
}
