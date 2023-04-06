using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dal.Migrations
{
    public partial class _0108a : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LikedUserStories",
                columns: table => new
                {
                    LeftId = table.Column<int>(nullable: false),
                    RightId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikedUserStories", x => new { x.LeftId, x.RightId });
                    table.ForeignKey(
                        name: "FK_LikedUserStories_Users_LeftId",
                        column: x => x.LeftId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LikedUserStories_Storyes_RightId",
                        column: x => x.RightId,
                        principalTable: "Storyes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ViewedUserStories",
                columns: table => new
                {
                    LeftId = table.Column<int>(nullable: false),
                    RightId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewedUserStories", x => new { x.LeftId, x.RightId });
                    table.ForeignKey(
                        name: "FK_ViewedUserStories_Users_LeftId",
                        column: x => x.LeftId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ViewedUserStories_Storyes_RightId",
                        column: x => x.RightId,
                        principalTable: "Storyes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikedUserStories_RightId",
                table: "LikedUserStories",
                column: "RightId");

            migrationBuilder.CreateIndex(
                name: "IX_ViewedUserStories_RightId",
                table: "ViewedUserStories",
                column: "RightId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikedUserStories");

            migrationBuilder.DropTable(
                name: "ViewedUserStories");
        }
    }
}
