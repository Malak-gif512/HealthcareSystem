using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Changes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatientProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BloodType = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LocationArea = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_PatientProfiles_PatientProfileId",
                        column: x => x.PatientProfileId,
                        principalTable: "PatientProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClinicalRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Diagnosis = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ClinicalNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClinicalRecords_PatientProfiles_PatientProfileId",
                        column: x => x.PatientProfileId,
                        principalTable: "PatientProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientProfileId",
                table: "Appointments",
                column: "PatientProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalRecords_PatientProfileId",
                table: "ClinicalRecords",
                column: "PatientProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientProfiles_UserId",
                table: "PatientProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ClinicalRecords");

            migrationBuilder.DropTable(
                name: "PatientProfiles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
