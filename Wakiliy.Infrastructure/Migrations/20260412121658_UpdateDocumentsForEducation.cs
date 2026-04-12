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
                name: "AcademicQualificationId",
                table: "UploadedFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_AcademicQualificationId",
                table: "UploadedFiles",
                column: "AcademicQualificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadedFiles_AcademicQualifications_AcademicQualificationId",
                table: "UploadedFiles",
                column: "AcademicQualificationId",
                principalTable: "AcademicQualifications",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadedFiles_AcademicQualifications_AcademicQualificationId",
                table: "UploadedFiles");

            migrationBuilder.DropIndex(
                name: "IX_UploadedFiles_AcademicQualificationId",
                table: "UploadedFiles");

            migrationBuilder.DropColumn(
                name: "AcademicQualificationId",
                table: "UploadedFiles");
        }
    }
}
