using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectV2.Migrations
{
    public partial class AddCalendar2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalendarId",
                table: "Venue");

            migrationBuilder.AddColumn<int>(
                name: "VenueId",
                table: "Calendar",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VenueId",
                table: "Calendar");

            migrationBuilder.AddColumn<int>(
                name: "CalendarId",
                table: "Venue",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
