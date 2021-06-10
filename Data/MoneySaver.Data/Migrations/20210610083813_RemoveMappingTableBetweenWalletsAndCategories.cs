using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Data.Migrations
{
    public partial class RemoveMappingTableBetweenWalletsAndCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Records_Wallets_WalletId",
                table: "Records");

            migrationBuilder.DropTable(
                name: "WalletsCategories");

            migrationBuilder.DropIndex(
                name: "IX_Records_WalletId",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "Records");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_WalletId",
                table: "Categories",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Wallets_WalletId",
                table: "Categories",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Wallets_WalletId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_WalletId",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "WalletId",
                table: "Records",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WalletsCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    WalletId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletsCategories", x => new { x.CategoryId, x.WalletId });
                    table.ForeignKey(
                        name: "FK_WalletsCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WalletsCategories_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Records_WalletId",
                table: "Records",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletsCategories_WalletId",
                table: "WalletsCategories",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Records_Wallets_WalletId",
                table: "Records",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
