using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PoliceManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_Agents_SuperiorId",
                table: "Agents");

            migrationBuilder.DropForeignKey(
                name: "FK_Agents_Conferences_ConferenceId",
                table: "Agents");

            migrationBuilder.DropForeignKey(
                name: "FK_Agents_PoliceStations_PoliceStationId",
                table: "Agents");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentTransfers_Agents_AgentId",
                table: "AgentTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentTransfers_PoliceStations_FromStationId",
                table: "AgentTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentTransfers_PoliceStations_ToStationId",
                table: "AgentTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Conferences_Agents_OrganizerId",
                table: "Conferences");

            migrationBuilder.DropForeignKey(
                name: "FK_CriminalFiles_Agents_AgentId",
                table: "CriminalFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CriminalFiles_PoliceStations_PoliceStationId",
                table: "CriminalFiles");

            migrationBuilder.DropIndex(
                name: "IX_Agents_ConferenceId",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "ConferenceId",
                table: "Agents");

            migrationBuilder.CreateTable(
                name: "AgentConference",
                columns: table => new
                {
                    ConferenceId = table.Column<int>(type: "int", nullable: false),
                    ParticipantsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentConference", x => new { x.ConferenceId, x.ParticipantsId });
                    table.ForeignKey(
                        name: "FK_AgentConference_Agents_ParticipantsId",
                        column: x => x.ParticipantsId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgentConference_Conferences_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityId = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OldValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NewValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Success = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IpAddress = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AgentConference_ParticipantsId",
                table: "AgentConference",
                column: "ParticipantsId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AgentId",
                table: "Users",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_Agents_SuperiorId",
                table: "Agents",
                column: "SuperiorId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_PoliceStations_PoliceStationId",
                table: "Agents",
                column: "PoliceStationId",
                principalTable: "PoliceStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentTransfers_Agents_AgentId",
                table: "AgentTransfers",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentTransfers_PoliceStations_FromStationId",
                table: "AgentTransfers",
                column: "FromStationId",
                principalTable: "PoliceStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentTransfers_PoliceStations_ToStationId",
                table: "AgentTransfers",
                column: "ToStationId",
                principalTable: "PoliceStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conferences_Agents_OrganizerId",
                table: "Conferences",
                column: "OrganizerId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CriminalFiles_Agents_AgentId",
                table: "CriminalFiles",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CriminalFiles_PoliceStations_PoliceStationId",
                table: "CriminalFiles",
                column: "PoliceStationId",
                principalTable: "PoliceStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agents_Agents_SuperiorId",
                table: "Agents");

            migrationBuilder.DropForeignKey(
                name: "FK_Agents_PoliceStations_PoliceStationId",
                table: "Agents");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentTransfers_Agents_AgentId",
                table: "AgentTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentTransfers_PoliceStations_FromStationId",
                table: "AgentTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_AgentTransfers_PoliceStations_ToStationId",
                table: "AgentTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Conferences_Agents_OrganizerId",
                table: "Conferences");

            migrationBuilder.DropForeignKey(
                name: "FK_CriminalFiles_Agents_AgentId",
                table: "CriminalFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_CriminalFiles_PoliceStations_PoliceStationId",
                table: "CriminalFiles");

            migrationBuilder.DropTable(
                name: "AgentConference");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.AddColumn<int>(
                name: "ConferenceId",
                table: "Agents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agents_ConferenceId",
                table: "Agents",
                column: "ConferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_Agents_SuperiorId",
                table: "Agents",
                column: "SuperiorId",
                principalTable: "Agents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_Conferences_ConferenceId",
                table: "Agents",
                column: "ConferenceId",
                principalTable: "Conferences",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agents_PoliceStations_PoliceStationId",
                table: "Agents",
                column: "PoliceStationId",
                principalTable: "PoliceStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentTransfers_Agents_AgentId",
                table: "AgentTransfers",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentTransfers_PoliceStations_FromStationId",
                table: "AgentTransfers",
                column: "FromStationId",
                principalTable: "PoliceStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AgentTransfers_PoliceStations_ToStationId",
                table: "AgentTransfers",
                column: "ToStationId",
                principalTable: "PoliceStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conferences_Agents_OrganizerId",
                table: "Conferences",
                column: "OrganizerId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CriminalFiles_Agents_AgentId",
                table: "CriminalFiles",
                column: "AgentId",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CriminalFiles_PoliceStations_PoliceStationId",
                table: "CriminalFiles",
                column: "PoliceStationId",
                principalTable: "PoliceStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
