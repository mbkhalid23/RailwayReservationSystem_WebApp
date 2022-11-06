using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayReservationSystem.DataAccess.Migrations
{
    public partial class RenameInverseProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Stations_FromStationId",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Stations_ToStationId",
                table: "Schedule");

            migrationBuilder.RenameColumn(
                name: "ToStationId",
                table: "Schedule",
                newName: "SourceStationId");

            migrationBuilder.RenameColumn(
                name: "FromStationId",
                table: "Schedule",
                newName: "DestinationStationId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_ToStationId",
                table: "Schedule",
                newName: "IX_Schedule_SourceStationId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_FromStationId",
                table: "Schedule",
                newName: "IX_Schedule_DestinationStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Stations_DestinationStationId",
                table: "Schedule",
                column: "DestinationStationId",
                principalTable: "Stations",
                principalColumn: "StationId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Stations_SourceStationId",
                table: "Schedule",
                column: "SourceStationId",
                principalTable: "Stations",
                principalColumn: "StationId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Stations_DestinationStationId",
                table: "Schedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedule_Stations_SourceStationId",
                table: "Schedule");

            migrationBuilder.RenameColumn(
                name: "SourceStationId",
                table: "Schedule",
                newName: "ToStationId");

            migrationBuilder.RenameColumn(
                name: "DestinationStationId",
                table: "Schedule",
                newName: "FromStationId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_SourceStationId",
                table: "Schedule",
                newName: "IX_Schedule_ToStationId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedule_DestinationStationId",
                table: "Schedule",
                newName: "IX_Schedule_FromStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Stations_FromStationId",
                table: "Schedule",
                column: "FromStationId",
                principalTable: "Stations",
                principalColumn: "StationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedule_Stations_ToStationId",
                table: "Schedule",
                column: "ToStationId",
                principalTable: "Stations",
                principalColumn: "StationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
