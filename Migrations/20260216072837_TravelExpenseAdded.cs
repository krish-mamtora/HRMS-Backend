using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS_Backend.Migrations
{
    /// <inheritdoc />
    public partial class TravelExpenseAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TravelExpense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelAssignId = table.Column<int>(type: "int", nullable: false),
                    ExpenseType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HrRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedBy = table.Column<int>(type: "int", nullable: true),
                    TravelAssignmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelExpense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelExpense_ExpensePolicy_ExpenseType",
                        column: x => x.ExpenseType,
                        principalTable: "ExpensePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TravelExpense_TravelAssignment_TravelAssignmentId",
                        column: x => x.TravelAssignmentId,
                        principalTable: "TravelAssignment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TravelExpense_Users_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TravelExpense_ApprovedBy",
                table: "TravelExpense",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TravelExpense_ExpenseType",
                table: "TravelExpense",
                column: "ExpenseType");

            migrationBuilder.CreateIndex(
                name: "IX_TravelExpense_TravelAssignmentId",
                table: "TravelExpense",
                column: "TravelAssignmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TravelExpense");
        }
    }
}
