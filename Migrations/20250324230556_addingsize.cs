using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothingAPIs.Migrations
{
    /// <inheritdoc />
    public partial class addingsize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "size",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "size",
                table: "Products");
        }
    }
}
