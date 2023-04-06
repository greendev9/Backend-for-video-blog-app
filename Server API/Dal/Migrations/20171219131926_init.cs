using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dal.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Body = table.Column<string>(maxLength: 600, nullable: true),
                    PublishDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Countryes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FatherId = table.Column<int>(maxLength: 100, nullable: true),
                    Index = table.Column<int>(nullable: false),
                    IsSpecial = table.Column<bool>(nullable: false),
                    Value = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countryes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ItemColors",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ColorHex = table.Column<string>(maxLength: 100, nullable: true),
                    FatherId = table.Column<int>(nullable: true),
                    FatherName = table.Column<string>(nullable: true),
                    Index = table.Column<int>(nullable: false),
                    IsSpecial = table.Column<bool>(nullable: false),
                    Value = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemColors", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationCaruselItems",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ImageName = table.Column<string>(maxLength: 100, nullable: true),
                    Index = table.Column<int>(nullable: false),
                    Text = table.Column<string>(maxLength: 800, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationCaruselItems", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FatherId = table.Column<int>(nullable: true),
                    Index = table.Column<int>(nullable: false),
                    IsSpecial = table.Column<bool>(nullable: false),
                    Value = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CountryId = table.Column<int>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CultureId = table.Column<int>(nullable: false),
                    Email = table.Column<string>(maxLength: 70, nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    ItemColorID = table.Column<int>(nullable: true),
                    LastLoginDate = table.Column<DateTime>(nullable: false),
                    NickName = table.Column<string>(maxLength: 100, nullable: true),
                    Password = table.Column<string>(maxLength: 70, nullable: true),
                    YearOfBirth = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Users_Countryes_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countryes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_ItemColors_ItemColorID",
                        column: x => x.ItemColorID,
                        principalTable: "ItemColors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Storyes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Body = table.Column<string>(maxLength: 1000, nullable: true),
                    Description = table.Column<string>(maxLength: 300, nullable: true),
                    IsCommentsAlowed = table.Column<bool>(nullable: false),
                    IsDraft = table.Column<bool>(nullable: false),
                    PublishDate = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: true),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Storyes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Storyes_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
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

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    LeftId = table.Column<int>(nullable: false),
                    RightId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.LeftId, x.RightId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_LeftId",
                        column: x => x.LeftId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RightId",
                        column: x => x.RightId,
                        principalTable: "Roles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_StoryComments_RightId",
                table: "StoryComments",
                column: "RightId");

            migrationBuilder.CreateIndex(
                name: "IX_Storyes_UserID",
                table: "Storyes",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserComments_RightId",
                table: "UserComments",
                column: "RightId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RightId",
                table: "UserRoles",
                column: "RightId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CountryId",
                table: "Users",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ItemColorID",
                table: "Users",
                column: "ItemColorID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrationCaruselItems");

            migrationBuilder.DropTable(
                name: "StoryComments");

            migrationBuilder.DropTable(
                name: "UserComments");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Storyes");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Countryes");

            migrationBuilder.DropTable(
                name: "ItemColors");
        }
    }
}
