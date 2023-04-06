using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dal.Migrations
{
    public partial class _0125 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageUsers",
                table: "MessageUsers");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "MessageUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageUsers",
                table: "MessageUsers",
                columns: new[] { "LeftId", "RightId", "ID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageUsers",
                table: "MessageUsers");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "MessageUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageUsers",
                table: "MessageUsers",
                columns: new[] { "LeftId", "RightId" });
        }
    }
}
