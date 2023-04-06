using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dal.Migrations
{
    public partial class _0108c : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoryComments");

            migrationBuilder.DropTable(
                name: "UserComments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoryComments",
                columns: table => new
                {
                    LeftId = table.Column<int>(nullable: false),
                    RightId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryComments", x => new { x.LeftId, x.RightId });
                    table.ForeignKey(
                        name: "FK_StoryComments_Storyes_LeftId",
                        column: x => x.LeftId,
                        principalTable: "Storyes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoryComments_Comments_RightId",
                        column: x => x.RightId,
                        principalTable: "Comments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserComments",
                columns: table => new
                {
                    LeftId = table.Column<int>(nullable: false),
                    RightId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserComments", x => new { x.LeftId, x.RightId });
                    table.ForeignKey(
                        name: "FK_UserComments_Users_LeftId",
                        column: x => x.LeftId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserComments_Comments_RightId",
                        column: x => x.RightId,
                        principalTable: "Comments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoryComments_RightId",
                table: "StoryComments",
                column: "RightId");

            migrationBuilder.CreateIndex(
                name: "IX_UserComments_RightId",
                table: "UserComments",
                column: "RightId");
        }
    }
}
