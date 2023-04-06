using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dal.Migrations
{
    public partial class _0123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TailedUsers",
                columns: table => new
                {
                    LeftId = table.Column<int>(nullable: false),
                    RightId = table.Column<int>(nullable: false),
                    OUserID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TailedUsers", x => new { x.LeftId, x.RightId });
                    table.ForeignKey(
                        name: "FK_TailedUsers_Users_LeftId",
                        column: x => x.LeftId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TailedUsers_Users_OUserID",
                        column: x => x.OUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TailedUsers_OUserID",
                table: "TailedUsers",
                column: "OUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TailedUsers");
        }
    }
}
