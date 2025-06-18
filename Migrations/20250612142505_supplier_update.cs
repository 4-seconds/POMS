using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseOrderManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class supplier_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Suppliers",
                newName: "PaymentMethod2");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Suppliers",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Suppliers",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod1",
                table: "Suppliers",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod1BankId",
                table: "Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod2BankId",
                table: "Suppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Suppliers",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Suppliers",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TinNumberFilePath",
                table: "Suppliers",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TradeLicenseFilePath",
                table: "Suppliers",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "PaymentMethod1",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "PaymentMethod1BankId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "PaymentMethod2BankId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "TinNumberFilePath",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "TradeLicenseFilePath",
                table: "Suppliers");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod2",
                table: "Suppliers",
                newName: "Address");
        }
    }
}
