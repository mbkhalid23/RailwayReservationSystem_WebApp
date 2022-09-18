using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayReservationSystem.DataAccess.Migrations
{
    public partial class AddTrainName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trains_Stations_StationId",
                table: "Trains");

            migrationBuilder.AlterColumn<int>(
                name: "StationId",
                table: "Trains",
                type: "int",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Trains",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Trains_Stations_StationId",
                table: "Trains",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "StationId",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trains_Stations_StationId",
                table: "Trains");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Trains");

            migrationBuilder.AlterColumn<int>(
                name: "StationId",
                table: "Trains",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Trains_Stations_StationId",
                table: "Trains",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "StationId");
        }
    }
}
