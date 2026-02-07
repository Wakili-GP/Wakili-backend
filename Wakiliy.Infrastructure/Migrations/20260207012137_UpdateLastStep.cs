using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wakiliy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLastStep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VerificationDocuments_LawyerId",
                table: "VerificationDocuments");

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

            migrationBuilder.RenameColumn(
                name: "ProfessionalCertificatesUrls",
                table: "VerificationDocuments",
                newName: "Type");

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "VerificationDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VerificationDocuments_FileId",
                table: "VerificationDocuments",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationDocuments_LawyerId",
                table: "VerificationDocuments",
                column: "LawyerId");

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationDocuments_UploadedFiles_FileId",
                table: "VerificationDocuments",
                column: "FileId",
                principalTable: "UploadedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerificationDocuments_UploadedFiles_FileId",
                table: "VerificationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_VerificationDocuments_FileId",
                table: "VerificationDocuments");

            migrationBuilder.DropIndex(
                name: "IX_VerificationDocuments_LawyerId",
                table: "VerificationDocuments");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "VerificationDocuments");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "VerificationDocuments",
                newName: "ProfessionalCertificatesUrls");

            migrationBuilder.AddColumn<string>(
                name: "EducationalCertificatesUrls",
                table: "VerificationDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.CreateIndex(
                name: "IX_VerificationDocuments_LawyerId",
                table: "VerificationDocuments",
                column: "LawyerId",
                unique: true);
        }
    }
}
