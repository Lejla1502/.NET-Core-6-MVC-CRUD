using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyBookWeb.Migrations
{
    public partial class ChangeCarriesToCarrierInOrderHEader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Carries",
                table: "OrderHeaders",
                newName: "Carrier");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Carrier",
                table: "OrderHeaders",
                newName: "Carries");
        }
    }
}
