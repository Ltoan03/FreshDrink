using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshDrink.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveIsFeatured : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Drinks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Drinks");
        }
    }
}
