using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Data.Migrations
{
    public partial class BaseModelParentAdded : Migration
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
                name: "DeletedOn",
                table: "Wallets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Wallets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Wallets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Shoplists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Shoplists",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Shoplists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "Categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_IsDeleted",
                table: "Wallets",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Shoplists_IsDeleted",
                table: "Shoplists",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsDeleted",
                table: "Products",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsDeleted",
                table: "Categories",
                column: "IsDeleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallets_IsDeleted",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_Shoplists_IsDeleted",
                table: "Shoplists");

            migrationBuilder.DropIndex(
                name: "IX_Products_IsDeleted",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Categories_IsDeleted",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Shoplists");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Shoplists");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Shoplists");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "Categories");
        }
    }
}
