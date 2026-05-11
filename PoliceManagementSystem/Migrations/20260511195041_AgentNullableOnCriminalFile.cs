using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AgentNullableOnCriminalFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CriminalFiles_Agents_AgentId",
                table: "CriminalFiles");

            migrationBuilder.AlterColumn<int>(
                name: "AgentId",
                table: "CriminalFiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CriminalFiles_Agents_AgentId",
                table: "CriminalFiles",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CriminalFiles_Agents_AgentId",
                table: "CriminalFiles");

            migrationBuilder.AlterColumn<int>(
                name: "AgentId",
                table: "CriminalFiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CriminalFiles_Agents_AgentId",
                table: "CriminalFiles",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
