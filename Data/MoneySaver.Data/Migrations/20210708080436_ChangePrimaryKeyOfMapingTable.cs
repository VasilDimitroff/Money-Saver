using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Data.Migrations
{
    public partial class ChangePrimaryKeyOfMapingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersTrades",
                table: "UsersTrades");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "UsersTrades",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersTrades",
                table: "UsersTrades",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UsersTrades_ApplicationUserId",
                table: "UsersTrades",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersTrades",
                table: "UsersTrades");

            migrationBuilder.DropIndex(
                name: "IX_UsersTrades_ApplicationUserId",
                table: "UsersTrades");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "UsersTrades",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersTrades",
                table: "UsersTrades",
                columns: new[] { "ApplicationUserId", "StockId" });
        }
    }
}
