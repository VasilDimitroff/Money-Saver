using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Data.Migrations
{
    public partial class RemoveStockModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersTrades_Stocks_StockId",
                table: "UsersTrades");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_UsersTrades_StockId",
                table: "UsersTrades");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "UsersTrades");

            migrationBuilder.RenameColumn(
                name: "TradeDate",
                table: "UsersTrades",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "StockId",
                table: "UsersTrades",
                newName: "StockQuantity");

            migrationBuilder.AddColumn<string>(
                name: "CompanyTicker",
                table: "UsersTrades",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "UsersTrades",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "UsersTrades",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_UsersTrades_CompanyTicker",
                table: "UsersTrades",
                column: "CompanyTicker");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersTrades_Companies_CompanyTicker",
                table: "UsersTrades",
                column: "CompanyTicker",
                principalTable: "Companies",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersTrades_Companies_CompanyTicker",
                table: "UsersTrades");

            migrationBuilder.DropIndex(
                name: "IX_UsersTrades_CompanyTicker",
                table: "UsersTrades");

            migrationBuilder.DropColumn(
                name: "CompanyTicker",
                table: "UsersTrades");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "UsersTrades");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "UsersTrades");

            migrationBuilder.RenameColumn(
                name: "StockQuantity",
                table: "UsersTrades",
                newName: "StockId");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "UsersTrades",
                newName: "TradeDate");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "UsersTrades",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyTicker = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_Companies_CompanyTicker",
                        column: x => x.CompanyTicker,
                        principalTable: "Companies",
                        principalColumn: "Ticker",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersTrades_StockId",
                table: "UsersTrades",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_CompanyTicker",
                table: "Stocks",
                column: "CompanyTicker");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersTrades_Stocks_StockId",
                table: "UsersTrades",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
