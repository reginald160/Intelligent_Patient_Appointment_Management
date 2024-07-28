using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Modified_UserNotificationSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationType",
                table: "UserNotificationSettings");

            migrationBuilder.AddColumn<string>(
                name: "IsEmailCheck",
                table: "UserNotificationSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsSMSCheck",
                table: "UserNotificationSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsWhatapp",
                table: "UserNotificationSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEmailCheck",
                table: "UserNotificationSettings");

            migrationBuilder.DropColumn(
                name: "IsSMSCheck",
                table: "UserNotificationSettings");

            migrationBuilder.DropColumn(
                name: "IsWhatapp",
                table: "UserNotificationSettings");

            migrationBuilder.AddColumn<string>(
                name: "NotificationType",
                table: "UserNotificationSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
