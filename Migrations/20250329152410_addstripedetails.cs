using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothingAPIs.Migrations
{
    /// <inheritdoc />
    public partial class addstripedetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "stripedetails",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "stripedetails",
                table: "Orders");
        }
    }
}
