using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalBackend.Migrations
{
    /// <inheritdoc />
    public partial class Addnameonanappointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Appointments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Appointments");
        }
    }
}
