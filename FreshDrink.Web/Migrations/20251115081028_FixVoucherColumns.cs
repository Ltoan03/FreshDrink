using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreshDrink.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixVoucherColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountPercent",
                table: "Vouchers",
                newName: "Discount");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxDiscountAmount",
                table: "Vouchers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Vouchers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Vouchers");

            migrationBuilder.RenameColumn(
                name: "Discount",
                table: "Vouchers",
                newName: "DiscountPercent");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxDiscountAmount",
                table: "Vouchers",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
