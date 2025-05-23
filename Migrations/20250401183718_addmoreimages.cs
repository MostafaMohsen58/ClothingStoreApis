using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothingAPIs.Migrations
{
    /// <inheritdoc />
    public partial class addmoreimages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "additionalimages",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "additionalimages",
                table: "Products");
        }
    }
}
