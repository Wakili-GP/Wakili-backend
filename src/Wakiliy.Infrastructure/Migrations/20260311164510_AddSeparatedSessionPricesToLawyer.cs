using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wakiliy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeparatedSessionPricesToLawyer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Lawyers",
                newName: "PhoneSessionPrice");

            migrationBuilder.AddColumn<decimal>(
                name: "InOfficeSessionPrice",
                table: "Lawyers",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InOfficeSessionPrice",
                table: "Lawyers");

            migrationBuilder.RenameColumn(
                name: "PhoneSessionPrice",
                table: "Lawyers",
                newName: "Price");
        }
    }
}
