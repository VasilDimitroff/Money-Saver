using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Data.Migrations
{
    public partial class EditStocksForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Companies_CompanyTicker",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_CompanyTicker",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Stocks");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyTicker",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyTicker1",
                table: "Stocks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_CompanyTicker1",
                table: "Stocks",
                column: "CompanyTicker1");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Companies_CompanyTicker1",
                table: "Stocks",
                column: "CompanyTicker1",
                principalTable: "Companies",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Companies_CompanyTicker1",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_CompanyTicker1",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "CompanyTicker1",
                table: "Stocks");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyTicker",
                table: "Stocks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Name",
                table: "Companies",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
    }
}
