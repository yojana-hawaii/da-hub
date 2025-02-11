using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class locationUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_location_name",
                table: "Locations");

            migrationBuilder.CreateIndex(
                name: "ix_location_name",
                table: "Locations",
                columns: new[] { "LocationName", "SubLocation" },
                unique: true,
                filter: "[SubLocation] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_location_name",
                table: "Locations");

            migrationBuilder.CreateIndex(
                name: "ix_location_name",
                table: "Locations",
                column: "LocationName",
                unique: true);
        }
    }
}
