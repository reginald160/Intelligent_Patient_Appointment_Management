using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class chat_added_inten_and_RecievedMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "APIResponse",
                table: "ChatModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BotMessage",
                table: "ChatModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserIntent",
                table: "ChatModels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APIResponse",
                table: "ChatModels");

            migrationBuilder.DropColumn(
                name: "BotMessage",
                table: "ChatModels");

            migrationBuilder.DropColumn(
                name: "UserIntent",
                table: "ChatModels");
        }
    }
}
