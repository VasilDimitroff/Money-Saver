using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Data.Migrations
{
    public partial class structureEdited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Wallets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Wallets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Records",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Records");
        }
    }
}
