using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Data.Migrations
{
    public partial class ChangedForeignKeyTypeOfStockToCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Companies_CompanyTicker1",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_CompanyTicker1",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_IsDeleted",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "CompanyTicker1",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Stocks");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyTicker",
                table: "Stocks",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_CompanyTicker",
                table: "Stocks",
                column: "CompanyTicker");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Companies_CompanyTicker",
                table: "Stocks",
                column: "CompanyTicker",
                principalTable: "Companies",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Companies_CompanyTicker",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_CompanyTicker",
                table: "Stocks");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyTicker",
                table: "Stocks",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "CompanyTicker1",
                table: "Stocks",
                type: "nvarchar(450)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_CompanyTicker1",
                table: "Stocks",
                column: "CompanyTicker1");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_IsDeleted",
                table: "Stocks",
                column: "IsDeleted");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Companies_CompanyTicker1",
                table: "Stocks",
                column: "CompanyTicker1",
                principalTable: "Companies",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
