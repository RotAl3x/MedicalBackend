using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddFreeCabinetHoursAndDoctorFree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDoctorFree",
                table: "Appointments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFree",
                table: "Appointments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDoctorFree",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsFree",
                table: "Appointments");
        }
    }
}
