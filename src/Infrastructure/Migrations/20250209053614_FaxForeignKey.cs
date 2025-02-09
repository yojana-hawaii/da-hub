using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FaxForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Faxes_Departments_DepartmentId",
                table: "Faxes");

            migrationBuilder.DropForeignKey(
                name: "FK_Faxes_Locations_LocationId",
                table: "Faxes");

            migrationBuilder.AddForeignKey(
                name: "FK_Faxes_Departments_DepartmentId",
                table: "Faxes",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Faxes_Locations_LocationId",
                table: "Faxes",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Faxes_Departments_DepartmentId",
                table: "Faxes");

            migrationBuilder.DropForeignKey(
                name: "FK_Faxes_Locations_LocationId",
                table: "Faxes");

            migrationBuilder.AddForeignKey(
                name: "FK_Faxes_Departments_DepartmentId",
                table: "Faxes",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Faxes_Locations_LocationId",
                table: "Faxes",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
