using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Data.Migrations
{
    public partial class RecordInheritBaseDeletableModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Stocks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Stocks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Stocks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Stocks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Records",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Records",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_IsDeleted",
                table: "Stocks",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Records_IsDeleted",
                table: "Records",
                column: "IsDeleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stocks_IsDeleted",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Records_IsDeleted",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Records");
        }
    }
}
