using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class TravelAssign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TravelAssignment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TravelAssignment_UserId",
                table: "TravelAssignment",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TravelAssignment_Users_UserId",
                table: "TravelAssignment",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TravelAssignment_Users_UserId",
                table: "TravelAssignment");

            migrationBuilder.DropIndex(
                name: "IX_TravelAssignment_UserId",
                table: "TravelAssignment");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TravelAssignment");
        }
    }
}
