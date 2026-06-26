using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wakiliy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEarningsAndPayroll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PaymentTransactionId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Payrolls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LawyerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payrolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payrolls_Lawyers_LawyerId",
                        column: x => x.LawyerId,
                        principalTable: "Lawyers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LawyerEarnings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LawyerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PlatformFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PayrollId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LawyerEarnings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LawyerEarnings_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LawyerEarnings_Lawyers_LawyerId",
                        column: x => x.LawyerId,
                        principalTable: "Lawyers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LawyerEarnings_Payrolls_PayrollId",
                        column: x => x.PayrollId,
                        principalTable: "Payrolls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PaymentTransactionId",
                table: "Appointments",
                column: "PaymentTransactionId",
                unique: true,
                filter: "[PaymentTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LawyerEarnings_AppointmentId",
                table: "LawyerEarnings",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LawyerEarnings_LawyerId",
                table: "LawyerEarnings",
                column: "LawyerId");

            migrationBuilder.CreateIndex(
                name: "IX_LawyerEarnings_PayrollId",
                table: "LawyerEarnings",
                column: "PayrollId");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_LawyerId",
                table: "Payrolls",
                column: "LawyerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_PaymentTransactions_PaymentTransactionId",
                table: "Appointments",
                column: "PaymentTransactionId",
                principalTable: "PaymentTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_PaymentTransactions_PaymentTransactionId",
                table: "Appointments");

            migrationBuilder.DropTable(
                name: "LawyerEarnings");

            migrationBuilder.DropTable(
                name: "Payrolls");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PaymentTransactionId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PaymentTransactionId",
                table: "Appointments");
        }
    }
}
