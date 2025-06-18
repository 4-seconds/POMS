using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseOrderManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class fix_purchase_order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_AspNetUsers_RequestedBy",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_RequestedBy",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "RequestedBy",
                table: "PurchaseOrders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestedBy",
                table: "PurchaseOrders",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_RequestedBy",
                table: "PurchaseOrders",
                column: "RequestedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_AspNetUsers_RequestedBy",
                table: "PurchaseOrders",
                column: "RequestedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
