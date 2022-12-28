using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayReservationSystem.DataAccess.Migrations
{
    public partial class UpdateScheduleAndTrain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatsAvailable",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "SeatsBooked",
                table: "Trains");

            migrationBuilder.AddColumn<int>(
                name: "SeatsAvailable",
                table: "Schedule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeatsBooked",
                table: "Schedule",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatsAvailable",
                table: "Schedule");

            migrationBuilder.DropColumn(
                name: "SeatsBooked",
                table: "Schedule");

            migrationBuilder.AddColumn<int>(
                name: "SeatsAvailable",
                table: "Trains",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeatsBooked",
                table: "Trains",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
