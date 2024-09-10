using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PatientPropertyAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PaymentInvoices_PatientId",
                table: "PaymentInvoices",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentInvoices_Patients_PatientId",
                table: "PaymentInvoices",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentInvoices_Patients_PatientId",
                table: "PaymentInvoices");

            migrationBuilder.DropIndex(
                name: "IX_PaymentInvoices_PatientId",
                table: "PaymentInvoices");
        }
    }
}
