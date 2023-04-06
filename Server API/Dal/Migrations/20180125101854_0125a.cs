using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dal.Migrations
{
    public partial class _0125a : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_MessageUsers_Users_LeftId",
            //    table: "MessageUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageUsers_Users_OUserID",
                table: "MessageUsers");

            //migrationBuilder.DropIndex(
            //    name: "IX_MessageUsers_LeftId",
            //    table: "MessageUsers");

            migrationBuilder.DropIndex(
                name: "IX_MessageUsers_OUserID",
                table: "MessageUsers");

            migrationBuilder.DropColumn(
                name: "OUserID",
                table: "MessageUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OUserID",
                table: "MessageUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageUsers_LeftId",
                table: "MessageUsers",
                column: "LeftId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageUsers_OUserID",
                table: "MessageUsers",
                column: "OUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageUsers_Users_LeftId",
                table: "MessageUsers",
                column: "LeftId",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageUsers_Users_OUserID",
                table: "MessageUsers",
                column: "OUserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
