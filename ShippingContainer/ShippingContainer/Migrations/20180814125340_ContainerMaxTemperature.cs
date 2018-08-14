using Microsoft.EntityFrameworkCore.Migrations;

namespace ShippingContainer.Migrations
{
    public partial class ContainerMaxTemperature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "MaxTemperature",
                table: "Containers",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxTemperature",
                table: "Containers");
        }
    }
}
