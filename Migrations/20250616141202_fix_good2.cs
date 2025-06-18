using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseOrderManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class fix_good2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remark",
                table: "GoodsReceived");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "GoodsReceived",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
