using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Data.Migrations
{
    public partial class ListAndProductModelsRenamedAndMappingTableDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductsShoplists");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ToDoListId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ToDoListId",
                table: "Products",
                column: "ToDoListId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Shoplists_ToDoListId",
                table: "Products",
                column: "ToDoListId",
                principalTable: "Shoplists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Shoplists_ToDoListId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ToDoListId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ToDoListId",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ProductsShoplists",
                columns: table => new
                {
                    ShoplistId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsShoplists", x => new { x.ShoplistId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_ProductsShoplists_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductsShoplists_Shoplists_ShoplistId",
                        column: x => x.ShoplistId,
                        principalTable: "Shoplists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductsShoplists_ProductId",
                table: "ProductsShoplists",
                column: "ProductId");
        }
    }
}
