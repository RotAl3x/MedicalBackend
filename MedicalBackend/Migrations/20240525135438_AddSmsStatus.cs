using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddSmsStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sent",
                table: "SendSmsQueue");

            migrationBuilder.AddColumn<string>(
                name: "Sid",
                table: "SendSmsQueue",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "SendSmsQueue",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sid",
                table: "SendSmsQueue");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SendSmsQueue");

            migrationBuilder.AddColumn<bool>(
                name: "Sent",
                table: "SendSmsQueue",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
