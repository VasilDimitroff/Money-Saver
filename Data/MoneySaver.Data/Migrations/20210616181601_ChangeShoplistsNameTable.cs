using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneySaver.Data.Migrations
{
    public partial class ChangeShoplistsNameTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Shoplists_ToDoListId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Shoplists_AspNetUsers_ApplicationUserId",
                table: "Shoplists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shoplists",
                table: "Shoplists");

            migrationBuilder.RenameTable(
                name: "Shoplists",
                newName: "ToDoLists");

            migrationBuilder.RenameIndex(
                name: "IX_Shoplists_ApplicationUserId",
                table: "ToDoLists",
                newName: "IX_ToDoLists_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ToDoLists",
                table: "ToDoLists",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ToDoLists_ToDoListId",
                table: "Products",
                column: "ToDoListId",
                principalTable: "ToDoLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoLists_AspNetUsers_ApplicationUserId",
                table: "ToDoLists",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ToDoLists_ToDoListId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ToDoLists_AspNetUsers_ApplicationUserId",
                table: "ToDoLists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ToDoLists",
                table: "ToDoLists");

            migrationBuilder.RenameTable(
                name: "ToDoLists",
                newName: "Shoplists");

            migrationBuilder.RenameIndex(
                name: "IX_ToDoLists_ApplicationUserId",
                table: "Shoplists",
                newName: "IX_Shoplists_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shoplists",
                table: "Shoplists",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Shoplists_ToDoListId",
                table: "Products",
                column: "ToDoListId",
                principalTable: "Shoplists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shoplists_AspNetUsers_ApplicationUserId",
                table: "Shoplists",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
