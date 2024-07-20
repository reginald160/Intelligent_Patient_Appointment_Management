using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class locked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "PaymentInvoices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "PaymentInvoices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "IdentityHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "IdentityHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "ChatModels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "ChatModels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "appointmentJobScheduleModels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "appointmentJobScheduleModels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "AppointmentEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "AppointmentEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "AdminModels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "AdminModels",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "PaymentInvoices");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "PaymentInvoices");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "IdentityHistories");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "IdentityHistories");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "ChatModels");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "ChatModels");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "appointmentJobScheduleModels");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "appointmentJobScheduleModels");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "AppointmentEvents");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "AppointmentEvents");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "AdminModels");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "AdminModels");
        }
    }
}
