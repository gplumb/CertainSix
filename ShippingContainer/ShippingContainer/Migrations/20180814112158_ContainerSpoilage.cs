using Microsoft.EntityFrameworkCore.Migrations;

namespace ShippingContainer.Migrations
{
    public partial class ContainerSpoilage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSpoiled",
                table: "Containers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSpoiled",
                table: "Containers");
        }
    }
}
