using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatReservation.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RenameVenueId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_seats_venues_VenueId",
                table: "seats");

            migrationBuilder.RenameColumn(
                name: "VenueId",
                table: "seats",
                newName: "venue_id");

            migrationBuilder.RenameIndex(
                name: "IX_seats_VenueId",
                table: "seats",
                newName: "IX_seats_venue_id");

            migrationBuilder.AddForeignKey(
                name: "FK_seats_venues_venue_id",
                table: "seats",
                column: "venue_id",
                principalTable: "venues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_seats_venues_venue_id",
                table: "seats");

            migrationBuilder.RenameColumn(
                name: "venue_id",
                table: "seats",
                newName: "VenueId");

            migrationBuilder.RenameIndex(
                name: "IX_seats_venue_id",
                table: "seats",
                newName: "IX_seats_VenueId");

            migrationBuilder.AddForeignKey(
                name: "FK_seats_venues_VenueId",
                table: "seats",
                column: "VenueId",
                principalTable: "venues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
