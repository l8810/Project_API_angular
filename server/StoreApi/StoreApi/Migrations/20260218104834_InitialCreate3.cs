using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gifts_Users_WinerId",
                table: "Gifts");

            migrationBuilder.DropIndex(
                name: "IX_Gifts_WinerId",
                table: "Gifts");

            migrationBuilder.DropColumn(
                name: "WinerId",
                table: "Gifts");

            migrationBuilder.CreateIndex(
                name: "IX_Gifts_WinnerId",
                table: "Gifts",
                column: "WinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gifts_Users_WinnerId",
                table: "Gifts",
                column: "WinnerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gifts_Users_WinnerId",
                table: "Gifts");

            migrationBuilder.DropIndex(
                name: "IX_Gifts_WinnerId",
                table: "Gifts");

            migrationBuilder.AddColumn<int>(
                name: "WinerId",
                table: "Gifts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gifts_WinerId",
                table: "Gifts",
                column: "WinerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gifts_Users_WinerId",
                table: "Gifts",
                column: "WinerId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
