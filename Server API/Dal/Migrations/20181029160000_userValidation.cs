using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Data;

namespace Dal.Migrations
{
    public partial class userValidation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.AddColumn<bool>(
            name: "EmailConfirmed",
            table: "Users",
            nullable: true);

          migrationBuilder.AddColumn<string>(
            name: "EmailConfirmToken",
            table: "Users",
            nullable: true,
            type:"NVARCHAR(256)");
        }

    protected override void Down(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.DropColumn(
            name: "EmailConfirmed",
            table: "Users");

          migrationBuilder.DropColumn(
            name: "EmailConfirmToken",
            table: "Users");
        }
  }
}
