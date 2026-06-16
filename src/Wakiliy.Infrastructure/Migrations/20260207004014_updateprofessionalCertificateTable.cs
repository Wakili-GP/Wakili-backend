using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wakiliy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateprofessionalCertificateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "ProfessionalCertifications");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "ProfessionalCertifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionalCertifications_DocumentId",
                table: "ProfessionalCertifications",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfessionalCertifications_UploadedFiles_DocumentId",
                table: "ProfessionalCertifications",
                column: "DocumentId",
                principalTable: "UploadedFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfessionalCertifications_UploadedFiles_DocumentId",
                table: "ProfessionalCertifications");

            migrationBuilder.DropIndex(
                name: "IX_ProfessionalCertifications_DocumentId",
                table: "ProfessionalCertifications");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "ProfessionalCertifications");

            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "ProfessionalCertifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
