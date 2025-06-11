using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce_website.Migrations
{
    public partial class order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    order_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<int>(nullable: false),
                    ord_name = table.Column<string>(nullable: false),
                    ord_email = table.Column<string>(nullable: false),
                    ord_phone = table.Column<int>(nullable: false),
                    state_id = table.Column<int>(nullable: false),
                    ord_address = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_orders_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Cascade); // Cascade allowed here
                    table.ForeignKey(
                        name: "FK_orders_states_state_id",
                        column: x => x.state_id,
                        principalTable: "states",
                        principalColumn: "state_id",
                        onDelete: ReferentialAction.Restrict); // Changed from CASCADE to RESTRICT
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_state_id",
                table: "orders",
                column: "state_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orders");
        }
    }
}
