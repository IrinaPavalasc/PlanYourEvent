using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectV2.Migrations
{
    public partial class AddUserVenue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProviderId",
                table: "Venue",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Venue_AspNetUsers_ProviderId",
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
    }
}
