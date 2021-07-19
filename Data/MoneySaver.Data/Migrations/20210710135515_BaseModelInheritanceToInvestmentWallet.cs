using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Data.Migrations
{
    public partial class BaseModelInheritanceToInvestmentWallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "InvestmentWallets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "InvestmentWallets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "InvestmentWallets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "InvestmentWallets");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "InvestmentWallets");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "InvestmentWallets");
        }
    }
}
