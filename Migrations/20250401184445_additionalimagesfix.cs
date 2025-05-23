using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothingAPIs.Migrations
{
    /// <inheritdoc />
    public partial class additionalimagesfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "additionalimages",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "ProductAdditionalImage",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    img = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAdditionalImage", x => x.id);
                    table.ForeignKey(
                        name: "FK_ProductAdditionalImage_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAdditionalImage_ProductId",
                table: "ProductAdditionalImage",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAdditionalImage");

            migrationBuilder.AddColumn<string>(
                name: "additionalimages",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
