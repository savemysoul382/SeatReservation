using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatReservation.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddUsers_JsonB_info : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Socials",
                table: "users",
                newName: "socials");

            migrationBuilder.AlterColumn<string>(
                name: "socials",
                table: "users",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(IReadOnlyList<SocialNetwork>),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "socials",
                table: "users",
                newName: "Socials");

            migrationBuilder.AlterColumn<IReadOnlyList<SocialNetwork>>(
                name: "Socials",
                table: "users",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
