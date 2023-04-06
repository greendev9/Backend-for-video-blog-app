using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dal.Migrations
{
    public partial class _0124 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockedUsers",
                columns: table => new
                {
                    LeftId = table.Column<int>(nullable: false),
                    RightId = table.Column<int>(nullable: false),
                    OUserID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedUsers", x => new { x.LeftId, x.RightId });
                    table.ForeignKey(
                        name: "FK_BlockedUsers_Users_LeftId",
                        column: x => x.LeftId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockedUsers_Users_OUserID",
                        column: x => x.OUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HidenUsers",
                columns: table => new
                {
                    LeftId = table.Column<int>(nullable: false),
                    RightId = table.Column<int>(nullable: false),
                    OUserID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HidenUsers", x => new { x.LeftId, x.RightId });
                    table.ForeignKey(
                        name: "FK_HidenUsers_Users_LeftId",
                        column: x => x.LeftId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HidenUsers_Users_OUserID",
                        column: x => x.OUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageUsers",
                columns: table => new
                {
                    LeftId = table.Column<int>(nullable: false),
                    RightId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(maxLength: 1000, nullable: true),
                    MessageDate = table.Column<DateTime>(nullable: false),
                    OUserID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageUsers", x => new { x.LeftId, x.RightId });
                    table.ForeignKey(
                        name: "FK_MessageUsers_Users_LeftId",
                        column: x => x.LeftId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageUsers_Users_OUserID",
                        column: x => x.OUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockedUsers_OUserID",
                table: "BlockedUsers",
                column: "OUserID");

            migrationBuilder.CreateIndex(
                name: "IX_HidenUsers_OUserID",
                table: "HidenUsers",
                column: "OUserID");

            migrationBuilder.CreateIndex(
                name: "IX_MessageUsers_OUserID",
                table: "MessageUsers",
                column: "OUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockedUsers");

            migrationBuilder.DropTable(
                name: "HidenUsers");

            migrationBuilder.DropTable(
                name: "MessageUsers");
        }
    }
}
