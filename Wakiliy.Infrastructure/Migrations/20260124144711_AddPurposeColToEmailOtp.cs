using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wakiliy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurposeColToEmailOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Purpose",
                table: "EmailOtps",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "EmailOtps");
        }
    }
}
