using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wakiliy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovedByAndRejectByForLawyer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Lawyers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedById",
                table: "Lawyers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Lawyers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedById",
                table: "Lawyers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lawyers_ApprovedById",
                table: "Lawyers",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Lawyers_RejectedById",
                table: "Lawyers",
                column: "RejectedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Lawyers_AspNetUsers_ApprovedById",
                table: "Lawyers",
                column: "ApprovedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lawyers_AspNetUsers_RejectedById",
                table: "Lawyers",
                column: "RejectedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lawyers_AspNetUsers_ApprovedById",
                table: "Lawyers");

            migrationBuilder.DropForeignKey(
                name: "FK_Lawyers_AspNetUsers_RejectedById",
                table: "Lawyers");

            migrationBuilder.DropIndex(
                name: "IX_Lawyers_ApprovedById",
                table: "Lawyers");

            migrationBuilder.DropIndex(
                name: "IX_Lawyers_RejectedById",
                table: "Lawyers");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Lawyers");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "Lawyers");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Lawyers");

            migrationBuilder.DropColumn(
                name: "RejectedById",
                table: "Lawyers");
        }
    }
}
