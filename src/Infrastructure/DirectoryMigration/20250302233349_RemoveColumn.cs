using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.DirectoryMigration
{
    /// <inheritdoc />
    public partial class RemoveColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_employee_keyword",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Keyword",
                table: "Employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Keyword",
                table: "Employees",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_employee_keyword",
                table: "Employees",
                column: "Keyword");
        }
    }
}
