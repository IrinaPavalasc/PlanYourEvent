using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectV2.Migrations
{
    public partial class AddAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Venue_AspNetUsers_ProviderId",
                table: "Venue");

            migrationBuilder.DropForeignKey(
                name: "FK_Venue_Calendar_CalendarId",
                table: "Venue");

            migrationBuilder.DropTable(
                name: "Calendar");

            migrationBuilder.DropIndex(
                name: "IX_Venue_CalendarId",
                table: "Venue");

            migrationBuilder.DropIndex(
                name: "IX_Venue_ProviderId",
                table: "Venue");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderId",
                table: "Venue",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProviderId",
                table: "Venue",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Calendar",
                columns: table => new
                {
                    CalendarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    PricePerDay = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendar", x => x.CalendarId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Venue_CalendarId",
                table: "Venue",
                column: "CalendarId");

            migrationBuilder.CreateIndex(
                name: "IX_Venue_ProviderId",
                table: "Venue",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Venue_AspNetUsers_ProviderId",
                table: "Venue",
                column: "ProviderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Venue_Calendar_CalendarId",
                table: "Venue",
                column: "CalendarId",
                principalTable: "Calendar",
                principalColumn: "CalendarId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
