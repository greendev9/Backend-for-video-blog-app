using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dal.Migrations
{
    public partial class mp4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockedUsers_Users_RightId",
                table: "BlockedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_HidenUsers_Users_RightId",
                table: "HidenUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageUsers_Users_RightId",
                table: "MessageUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TailedUsers_Users_RightId",
                table: "TailedUsers");

            migrationBuilder.DropIndex(
                name: "IX_TailedUsers_RightId",
                table: "TailedUsers");

            migrationBuilder.DropIndex(
                name: "IX_MessageUsers_RightId",
                table: "MessageUsers");

            migrationBuilder.DropIndex(
                name: "IX_HidenUsers_RightId",
                table: "HidenUsers");

            migrationBuilder.DropIndex(
                name: "IX_BlockedUsers_RightId",
                table: "BlockedUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedUsers_Users_LeftId",
                table: "BlockedUsers",
                column: "LeftId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HidenUsers_Users_LeftId",
                table: "HidenUsers",
                column: "LeftId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageUsers_Users_LeftId",
                table: "MessageUsers",
                column: "LeftId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_BlockedUsers_Users_LeftId",
                table: "BlockedUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_HidenUsers_Users_LeftId",
                table: "HidenUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageUsers_Users_LeftId",
                table: "MessageUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TailedUsers_Users_LeftId",
                table: "TailedUsers");

            migrationBuilder.CreateIndex(
                name: "IX_TailedUsers_RightId",
                table: "TailedUsers",
                column: "RightId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageUsers_RightId",
                table: "MessageUsers",
                column: "RightId");

            migrationBuilder.CreateIndex(
                name: "IX_HidenUsers_RightId",
                table: "HidenUsers",
                column: "RightId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockedUsers_RightId",
                table: "BlockedUsers",
                column: "RightId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockedUsers_Users_RightId",
                table: "BlockedUsers",
                column: "RightId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HidenUsers_Users_RightId",
                table: "HidenUsers",
                column: "RightId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageUsers_Users_RightId",
                table: "MessageUsers",
                column: "RightId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TailedUsers_Users_RightId",
                table: "TailedUsers",
                column: "RightId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
