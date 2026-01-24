using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatReservation.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Venue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SeatNumber",
                table: "seats",
                newName: "seat_number");

            migrationBuilder.RenameColumn(
                name: "RowNumber",
                table: "seats",
                newName: "row_number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "seat_number",
                table: "seats",
                newName: "SeatNumber");

            migrationBuilder.RenameColumn(
                name: "row_number",
                table: "seats",
                newName: "RowNumber");
        }
    }
}
