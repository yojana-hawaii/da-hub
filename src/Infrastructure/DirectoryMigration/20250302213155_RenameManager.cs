using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.DirectoryMigration
{
    /// <inheritdoc />
    public partial class RenameManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Employees_PrimaryManagerId",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "PrimaryManagerId",
                table: "Employees",
                newName: "ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_PrimaryManagerId",
                table: "Employees",
                newName: "IX_Employees_ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Employees_ManagerId",
                table: "Employees",
                column: "ManagerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Employees_ManagerId",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "Employees",
                newName: "PrimaryManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_ManagerId",
                table: "Employees",
                newName: "IX_Employees_PrimaryManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Employees_PrimaryManagerId",
                table: "Employees",
                column: "PrimaryManagerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
