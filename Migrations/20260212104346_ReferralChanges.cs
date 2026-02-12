using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class ReferralChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Referals_Jobs_JobsId",
                table: "Referals");

            migrationBuilder.DropIndex(
                name: "IX_Referals_JobsId",
                table: "Referals");

            migrationBuilder.DropColumn(
                name: "JobsId",
                table: "Referals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobsId",
                table: "Referals",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Referals_JobsId",
                table: "Referals",
                column: "JobsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Referals_Jobs_JobsId",
                table: "Referals",
                column: "JobsId",
                principalTable: "Jobs",
                principalColumn: "Id");
        }
    }
}
