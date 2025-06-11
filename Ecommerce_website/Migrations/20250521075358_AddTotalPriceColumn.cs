using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce_website.Migrations
{
    public partial class AddTotalPriceColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adding the total_price column to the 'carts' table
            migrationBuilder.AddColumn<int>(
                name: "total_price",
                table: "carts",
                nullable: false,
                defaultValue: 0); // Set default value as 0 (or your choice)
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // If the migration is rolled back, remove the total_price column
            migrationBuilder.DropColumn(
                name: "total_price",
                table: "carts");
        }
    }
}

