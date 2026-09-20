using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarmentWorkshop.Migrations
{
    /// <inheritdoc />
    public partial class AddStyleOrColorToWorkOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StyleOrColor",
                table: "WorkOrders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StyleOrColor",
                table: "WorkOrders");
        }
    }
}
