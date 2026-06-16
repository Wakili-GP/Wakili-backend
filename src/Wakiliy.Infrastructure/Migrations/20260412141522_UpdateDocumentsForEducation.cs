using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wakiliy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDocumentsForEducation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "AcademicQualifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicQualifications_DocumentId",
                table: "AcademicQualifications",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicQualifications_UploadedFiles_DocumentId",
                table: "AcademicQualifications",
                column: "DocumentId",
                principalTable: "UploadedFiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicQualifications_UploadedFiles_DocumentId",
                table: "AcademicQualifications");

            migrationBuilder.DropIndex(
                name: "IX_AcademicQualifications_DocumentId",
                table: "AcademicQualifications");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "AcademicQualifications");
        }
    }
}
