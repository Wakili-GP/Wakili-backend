using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wakiliy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addOnBoardingLawyer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionType",
                table: "Lawyers");

            migrationBuilder.AlterColumn<int>(
                name: "VerificationStatus",
                table: "Lawyers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "Lawyers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Lawyers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CompletedOnboardingSteps",
                table: "Lawyers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Lawyers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CurrentOnboardingStep",
                table: "Lawyers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOnboardingUpdate",
                table: "Lawyers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PracticeAreas",
                table: "Lawyers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "SessionTypes",
                table: "Lawyers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateTable(
                name: "AcademicQualifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LawyerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DegreeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniversityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GraduationYear = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicQualifications_Lawyers_LawyerId",
                        column: x => x.LawyerId,
                        principalTable: "Lawyers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfessionalCertifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LawyerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CertificateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuingOrganization = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearObtained = table.Column<int>(type: "int", nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalCertifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionalCertifications_Lawyers_LawyerId",
                        column: x => x.LawyerId,
                        principalTable: "Lawyers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerificationDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LawyerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NationalIdFront_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NationalIdFrontPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NationalIdFrontFileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NationalIdFrontStatus = table.Column<int>(type: "int", nullable: false),
                    NationalIdBack_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NationalIdBackPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NationalIdBackFileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NationalIdBackStatus = table.Column<int>(type: "int", nullable: false),
                    LawyerLicense_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LawyerLicensePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LawyerLicenseFileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LawyerLicenseStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationDocuments_Lawyers_LawyerId",
                        column: x => x.LawyerId,
                        principalTable: "Lawyers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LawyerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartYear = table.Column<int>(type: "int", nullable: false),
                    EndYear = table.Column<int>(type: "int", nullable: true),
                    IsCurrentJob = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkExperiences_Lawyers_LawyerId",
                        column: x => x.LawyerId,
                        principalTable: "Lawyers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerificationEducationalCertificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    VerificationDocumentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationEducationalCertificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationEducationalCertificates_VerificationDocuments_VerificationDocumentsId",
                        column: x => x.VerificationDocumentsId,
                        principalTable: "VerificationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerificationProfessionalCertificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    VerificationDocumentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationProfessionalCertificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationProfessionalCertificates_VerificationDocuments_VerificationDocumentsId",
                        column: x => x.VerificationDocumentsId,
                        principalTable: "VerificationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicQualifications_LawyerId",
                table: "AcademicQualifications",
                column: "LawyerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalCertifications_LawyerId",
                table: "ProfessionalCertifications",
                column: "LawyerId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationDocuments_LawyerId",
                table: "VerificationDocuments",
                column: "LawyerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VerificationEducationalCertificates_VerificationDocumentsId",
                table: "VerificationEducationalCertificates",
                column: "VerificationDocumentsId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationProfessionalCertificates_VerificationDocumentsId",
                table: "VerificationProfessionalCertificates",
                column: "VerificationDocumentsId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkExperiences_LawyerId",
                table: "WorkExperiences",
                column: "LawyerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicQualifications");

            migrationBuilder.DropTable(
                name: "ProfessionalCertifications");

            migrationBuilder.DropTable(
                name: "VerificationEducationalCertificates");

            migrationBuilder.DropTable(
                name: "VerificationProfessionalCertificates");

            migrationBuilder.DropTable(
                name: "WorkExperiences");

            migrationBuilder.DropTable(
                name: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Lawyers");

            migrationBuilder.DropColumn(
                name: "CompletedOnboardingSteps",
                table: "Lawyers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Lawyers");

            migrationBuilder.DropColumn(
                name: "CurrentOnboardingStep",
                table: "Lawyers");

            migrationBuilder.DropColumn(
                name: "LastOnboardingUpdate",
                table: "Lawyers");

            migrationBuilder.DropColumn(
                name: "PracticeAreas",
                table: "Lawyers");

            migrationBuilder.DropColumn(
                name: "SessionTypes",
                table: "Lawyers");

            migrationBuilder.AlterColumn<int>(
                name: "VerificationStatus",
                table: "Lawyers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "Lawyers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SessionType",
                table: "Lawyers",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
