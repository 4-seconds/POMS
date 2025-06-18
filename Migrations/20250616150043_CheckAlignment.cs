using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseOrderManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class CheckAlignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccountName",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "BankAccountNumber",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "Suppliers");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Suppliers",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Suppliers",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "Suppliers",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "Suppliers");

            migrationBuilder.AddColumn<string>(
                name: "BankAccountName",
                table: "Suppliers",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "BankAccountNumber",
                table: "Suppliers",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "BankCode",
                table: "Suppliers",
                type: "int",
                nullable: true);
        }
    }
}
