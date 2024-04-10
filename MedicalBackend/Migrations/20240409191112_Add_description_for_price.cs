using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalBackend.Migrations
{
    /// <inheritdoc />
    public partial class Add_description_for_price : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Prices",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Prices");
        }
    }
}
