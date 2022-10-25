using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectV2.Migrations
{
    public partial class update2Reservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_EventId",
                table: "Reservation",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Event_EventId",
                table: "Reservation",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Event_EventId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_EventId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Reservation");
        }
    }
}
