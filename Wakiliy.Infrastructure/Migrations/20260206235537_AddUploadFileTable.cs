using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wakiliy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUploadFileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerificationEducationalCertificates");

            migrationBuilder.DropTable(
                name: "VerificationProfessionalCertificates");

            migrationBuilder.DropColumn(
                name: "LawyerLicenseFileName",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "LawyerLicensePath",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "LawyerLicenseStatus",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "LawyerLicense_Id",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "NationalIdBackFileName",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "NationalIdBackPath",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "NationalIdBackStatus",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "NationalIdBack_Id",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "NationalIdFrontFileName",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "NationalIdFrontPath",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "NationalIdFrontStatus",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "NationalIdFront_Id",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "EducationalCertificatesUrls",
                table: "VerificationDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "LawyerLicenseUrl",
                table: "VerificationDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdBackUrl",
                table: "VerificationDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdFrontUrl",
                table: "VerificationDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfessionalCertificatesUrls",
                table: "VerificationDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadedFiles_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_OwnerId",
                table: "UploadedFiles",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UploadedFiles");

            migrationBuilder.DropColumn(
                name: "EducationalCertificatesUrls",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "LawyerLicenseUrl",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "NationalIdBackUrl",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "NationalIdFrontUrl",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "ProfessionalCertificatesUrls",
                table: "VerificationDocuments");

            migrationBuilder.AddColumn<string>(
                name: "LawyerLicenseFileName",
                table: "VerificationDocuments",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LawyerLicensePath",
                table: "VerificationDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LawyerLicenseStatus",
                table: "VerificationDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "LawyerLicense_Id",
                table: "VerificationDocuments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "NationalIdBackFileName",
                table: "VerificationDocuments",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdBackPath",
                table: "VerificationDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NationalIdBackStatus",
                table: "VerificationDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "NationalIdBack_Id",
                table: "VerificationDocuments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "NationalIdFrontFileName",
                table: "VerificationDocuments",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NationalIdFrontPath",
                table: "VerificationDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NationalIdFrontStatus",
                table: "VerificationDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "NationalIdFront_Id",
                table: "VerificationDocuments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VerificationEducationalCertificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
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
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
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
                name: "IX_VerificationEducationalCertificates_VerificationDocumentsId",
                table: "VerificationEducationalCertificates",
                column: "VerificationDocumentsId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationProfessionalCertificates_VerificationDocumentsId",
                table: "VerificationProfessionalCertificates",
                column: "VerificationDocumentsId");
        }
    }
}
