using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dal.Migrations
{
    public partial class mp2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TailedUsers_Users_IUserID",
                table: "TailedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TailedUsers_Users_UserID",
                table: "TailedUsers");

            migrationBuilder.DropIndex(
                name: "IX_TailedUsers_IUserID",
                table: "TailedUsers");

            migrationBuilder.DropIndex(
                name: "IX_TailedUsers_UserID",
                table: "TailedUsers");

            migrationBuilder.DropColumn(
                name: "IUserID",
                table: "TailedUsers");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "TailedUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_TailedUsers_Users_LeftId",
                table: "TailedUsers",
                column: "LeftId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TailedUsers_Users_LeftId",
                table: "TailedUsers");

            migrationBuilder.AddColumn<int>(
                name: "IUserID",
                table: "TailedUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "TailedUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TailedUsers_IUserID",
                table: "TailedUsers",
                column: "IUserID");

            migrationBuilder.CreateIndex(
                name: "IX_TailedUsers_UserID",
                table: "TailedUsers",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_TailedUsers_Users_IUserID",
                table: "TailedUsers",
                column: "IUserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TailedUsers_Users_UserID",
                table: "TailedUsers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
