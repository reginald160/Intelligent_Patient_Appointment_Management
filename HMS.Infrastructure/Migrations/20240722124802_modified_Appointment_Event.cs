using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modified_Appointment_Event : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentEvents_Appointments_AppointmentId",
                table: "AppointmentEvents");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentEvents_AppointmentId",
                table: "AppointmentEvents");

            migrationBuilder.RenameColumn(
                name: "AppointmentId",
                table: "AppointmentEvents",
                newName: "ObjectId");

            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentModelId",
                table: "AppointmentEvents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "JobDate",
                table: "AppointmentEvents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "JobId",
                table: "AppointmentEvents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "JobNotificationTime",
                table: "AppointmentEvents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "AppointmentEvents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentEvents_AppointmentModelId",
                table: "AppointmentEvents",
                column: "AppointmentModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentEvents_Appointments_AppointmentModelId",
                table: "AppointmentEvents",
                column: "AppointmentModelId",
                principalTable: "Appointments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentEvents_Appointments_AppointmentModelId",
                table: "AppointmentEvents");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentEvents_AppointmentModelId",
                table: "AppointmentEvents");

            migrationBuilder.DropColumn(
                name: "AppointmentModelId",
                table: "AppointmentEvents");

            migrationBuilder.DropColumn(
                name: "JobDate",
                table: "AppointmentEvents");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "AppointmentEvents");

            migrationBuilder.DropColumn(
                name: "JobNotificationTime",
                table: "AppointmentEvents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppointmentEvents");

            migrationBuilder.RenameColumn(
                name: "ObjectId",
                table: "AppointmentEvents",
                newName: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentEvents_AppointmentId",
                table: "AppointmentEvents",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentEvents_Appointments_AppointmentId",
                table: "AppointmentEvents",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");
        }
    }
}
