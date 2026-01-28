using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatReservation.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastReservationUtc",
                table: "events_details",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "events_details",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReservationUtc",
                table: "events_details");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "events_details");
        }
    }
}
