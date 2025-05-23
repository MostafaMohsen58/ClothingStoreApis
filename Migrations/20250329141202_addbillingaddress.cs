using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothingAPIs.Migrations
{
    /// <inheritdoc />
    public partial class addbillingaddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillignAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillignAddress",
                table: "Orders");
        }
    }
}
