using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wakiliy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSystemReviewAndAddAiModeration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemReviews");

            migrationBuilder.DropColumn(
                name: "AiAnalysis_Confidence",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "AiAnalysis_IsFlagged",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "AiAnalysis_Summary",
                table: "Reviews");

            migrationBuilder.AddColumn<string>(
                name: "AiComment",
                table: "Reviews",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AiConfidenceRate",
                table: "Reviews",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AiProcessedAt",
                table: "Reviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AiStatus",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Visibility",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiComment",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "AiConfidenceRate",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "AiProcessedAt",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "AiStatus",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Reviews");

            migrationBuilder.AddColumn<double>(
                name: "AiAnalysis_Confidence",
                table: "Reviews",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "AiAnalysis_IsFlagged",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AiAnalysis_Summary",
                table: "Reviews",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SystemReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    AiAnalysis_Confidence = table.Column<double>(type: "float", nullable: false),
                    AiAnalysis_IsFlagged = table.Column<bool>(type: "bit", nullable: false),
                    AiAnalysis_Summary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemReviews_Clients_UserId",
                        column: x => x.UserId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemReviews_UserId",
                table: "SystemReviews",
                column: "UserId");
        }
    }
}
