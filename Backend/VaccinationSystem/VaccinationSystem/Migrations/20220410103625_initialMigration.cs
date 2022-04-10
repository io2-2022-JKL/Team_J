using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VaccinationSystem.Migrations
{
    public partial class initialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PESEL = table.Column<string>(type: "varchar(11)", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(50)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(50)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    Mail = table.Column<string>(type: "varchar(100)", nullable: false),
                    Password = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PESEL = table.Column<string>(type: "varchar(11)", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(50)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(50)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    Mail = table.Column<string>(type: "varchar(100)", nullable: false),
                    Password = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaccinationCenter",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    City = table.Column<string>(type: "varchar(40)", nullable: false),
                    Address = table.Column<string>(type: "varchar(100)", nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaccinationCenter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Certificate",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Url = table.Column<string>(type: "varchar(250)", nullable: false),
                    PatientId = table.Column<Guid>(nullable: false),
                    VaccineId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificate_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Doctor",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    VaccinationCenterId = table.Column<Guid>(nullable: false),
                    PatientAccountId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doctor_Patient_PatientAccountId",
                        column: x => x.PatientAccountId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Doctor_VaccinationCenter_VaccinationCenterId",
                        column: x => x.VaccinationCenterId,
                        principalTable: "VaccinationCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpeningHours",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    From = table.Column<TimeSpan>(nullable: false),
                    To = table.Column<TimeSpan>(nullable: false),
                    VaccinationCenterId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpeningHours_VaccinationCenter_VaccinationCenterId",
                        column: x => x.VaccinationCenterId,
                        principalTable: "VaccinationCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vaccine",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Company = table.Column<string>(type: "varchar(50)", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    NumberOfDoses = table.Column<int>(nullable: false),
                    MinDaysBetweenDoses = table.Column<int>(nullable: false),
                    MaxDaysBetweenDoses = table.Column<int>(nullable: false),
                    Virus = table.Column<int>(nullable: false),
                    MinPatientAge = table.Column<int>(nullable: false),
                    MaxPatientAge = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    VaccinationCenterId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaccine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vaccine_VaccinationCenter_VaccinationCenterId",
                        column: x => x.VaccinationCenterId,
                        principalTable: "VaccinationCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlot",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    From = table.Column<DateTime>(nullable: false),
                    To = table.Column<DateTime>(nullable: false),
                    DoctorId = table.Column<Guid>(nullable: true),
                    IsFree = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeSlot_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WhichDose = table.Column<int>(nullable: false),
                    TimeSlotId = table.Column<Guid>(nullable: true),
                    PatientId = table.Column<Guid>(nullable: false),
                    VaccineId = table.Column<Guid>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    VaccineBatchNumber = table.Column<string>(type: "varchar(100)", nullable: true),
                    DoctorId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointment_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_Patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointment_TimeSlot_TimeSlotId",
                        column: x => x.TimeSlotId,
                        principalTable: "TimeSlot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_Vaccine_VaccineId",
                        column: x => x.VaccineId,
                        principalTable: "Vaccine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_DoctorId",
                table: "Appointment",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PatientId",
                table: "Appointment",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_TimeSlotId",
                table: "Appointment",
                column: "TimeSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_VaccineId",
                table: "Appointment",
                column: "VaccineId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_PatientId",
                table: "Certificate",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_PatientAccountId",
                table: "Doctor",
                column: "PatientAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_VaccinationCenterId",
                table: "Doctor",
                column: "VaccinationCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningHours_VaccinationCenterId",
                table: "OpeningHours",
                column: "VaccinationCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlot_DoctorId",
                table: "TimeSlot",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Vaccine_VaccinationCenterId",
                table: "Vaccine",
                column: "VaccinationCenterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "Certificate");

            migrationBuilder.DropTable(
                name: "OpeningHours");

            migrationBuilder.DropTable(
                name: "TimeSlot");

            migrationBuilder.DropTable(
                name: "Vaccine");

            migrationBuilder.DropTable(
                name: "Doctor");

            migrationBuilder.DropTable(
                name: "Patient");

            migrationBuilder.DropTable(
                name: "VaccinationCenter");
        }
    }
}
