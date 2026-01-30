using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatReservation.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "events_details",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "events_details",
                newName: "capacity");

            migrationBuilder.RenameColumn(
                name: "LastReservationUtc",
                table: "events_details",
                newName: "last_reservation_utc");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "events",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "events",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Info",
                table: "events",
                newName: "info");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "events",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "EventDate",
                table: "events",
                newName: "event_date");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "events",
                newName: "end_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "description",
                table: "events_details",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "capacity",
                table: "events_details",
                newName: "Capacity");

            migrationBuilder.RenameColumn(
                name: "last_reservation_utc",
                table: "events_details",
                newName: "LastReservationUtc");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "events",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "events",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "info",
                table: "events",
                newName: "Info");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "events",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "event_date",
                table: "events",
                newName: "EventDate");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "events",
                newName: "EndDate");
        }
    }
}
