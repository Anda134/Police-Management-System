using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_PoliceStations_PoliceStationId",
                table: "Agents");

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "CriminalFiles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_PoliceStations_PoliceStationId",
                table: "Agents",
                column: "PoliceStationId",
                principalTable: "PoliceStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_PoliceStations_PoliceStationId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "Details",
                table: "CriminalFiles");

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_PoliceStations_PoliceStationId",
                table: "Agents",
                column: "PoliceStationId",
                principalTable: "PoliceStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
