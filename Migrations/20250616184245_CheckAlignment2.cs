using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchaseOrderManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class CheckAlignment2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Suppliers_SupplierId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Items_ItemId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_PurchaseRequests_PurchaseRequestId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Suppliers_SupplierId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceived_Bids_BidId",
                table: "GoodsReceived");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceived_Items_ItemId",
                table: "GoodsReceived");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceived_PurchaseRequests_PurchaseRequestId",
                table: "GoodsReceived");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_AspNetUsers_CreatedBy",
                table: "PurchaseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_AspNetUsers_reviwed_by",
                table: "PurchaseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Items_existingItems",
                table: "PurchaseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierBranches_Suppliers_SupplierId",
                table: "SupplierBranches");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseRequests",
                table: "PurchaseRequests");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequests_CreatedBy",
                table: "PurchaseRequests");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequests_existingItems",
                table: "PurchaseRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CategoryId",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bids",
                table: "Bids");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SupplierId",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "15bb9e8f-9934-4ec4-9f30-e5e0a9ffe5ea", "d8a17e37-3489-4a0c-a577-98a185befbda" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "15bb9e8f-9934-4ec4-9f30-e5e0a9ffe5ea");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d8a17e37-3489-4a0c-a577-98a185befbda");

            migrationBuilder.DropColumn(
                name: "purchaseRequestId",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "Item",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "ItemDescription",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "ReviewdComment",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "existingItem",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "existingItems",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "remark",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CurrentStock",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UOM",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "GoodsReceived");

            migrationBuilder.DropColumn(
                name: "UOM",
                table: "GoodsReceived");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SupplierID",
                table: "Suppliers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "reviwed_by",
                table: "PurchaseRequests",
                newName: "ExistingItemId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "PurchaseRequests",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseRequests_reviwed_by",
                table: "PurchaseRequests",
                newName: "IX_PurchaseRequests_ExistingItemId");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Items",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "Bids",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "BidId",
                table: "Bids",
                newName: "AuctionId");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Suppliers",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Suppliers",
                type: "varchar(100)",
                maxLength: 100,
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

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod2",
                table: "Suppliers",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.AddColumn<string>(
                name: "BudgetComment",
                table: "PurchaseRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReviewedComment",
                table: "PurchaseRequests",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Items",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Items",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "GoodsReceived",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "GoodsReceived",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "GoodsReceived",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Bids",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Bids",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseRequests",
                table: "PurchaseRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bids",
                table: "Bids",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PurchaseRequestId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DeliveryDeadline = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auctions_PurchaseRequests_PurchaseRequestId",
                        column: x => x.PurchaseRequestId,
                        principalTable: "PurchaseRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequestId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BidId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OrderedBy = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Unit = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<float>(type: "float", nullable: false),
                    TotalPrice = table.Column<float>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_AspNetUsers_OrderedBy",
                        column: x => x.OrderedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Bids_BidId",
                        column: x => x.BidId,
                        principalTable: "Bids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_PurchaseRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "PurchaseRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentTransfers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PurchaseOrderId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InitiatedById = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Reference = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BankCode = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TransactionId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransfers_AspNetUsers_InitiatedById",
                        column: x => x.InitiatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentTransfers_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_UserId",
                table: "Suppliers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bids_AuctionId",
                table: "Bids",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_PurchaseRequestId",
                table: "Auctions",
                column: "PurchaseRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransfers_InitiatedById",
                table: "PaymentTransfers",
                column: "InitiatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransfers_PurchaseOrderId",
                table: "PaymentTransfers",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_BidId",
                table: "PurchaseOrders",
                column: "BidId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_OrderedBy",
                table: "PurchaseOrders",
                column: "OrderedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_RequestId",
                table: "PurchaseOrders",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Auctions_AuctionId",
                table: "Bids",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Items_ItemId",
                table: "Bids",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_PurchaseRequests_PurchaseRequestId",
                table: "Bids",
                column: "PurchaseRequestId",
                principalTable: "PurchaseRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Suppliers_SupplierId",
                table: "Bids",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceived_Bids_BidId",
                table: "GoodsReceived",
                column: "BidId",
                principalTable: "Bids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceived_Items_ItemId",
                table: "GoodsReceived",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceived_PurchaseRequests_PurchaseRequestId",
                table: "GoodsReceived",
                column: "PurchaseRequestId",
                principalTable: "PurchaseRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Items_ExistingItemId",
                table: "PurchaseRequests",
                column: "ExistingItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierBranches_Suppliers_SupplierId",
                table: "SupplierBranches",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_AspNetUsers_UserId",
                table: "Suppliers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Auctions_AuctionId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Items_ItemId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_PurchaseRequests_PurchaseRequestId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Suppliers_SupplierId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceived_Bids_BidId",
                table: "GoodsReceived");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceived_Items_ItemId",
                table: "GoodsReceived");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceived_PurchaseRequests_PurchaseRequestId",
                table: "GoodsReceived");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseRequests_Items_ExistingItemId",
                table: "PurchaseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierBranches_Suppliers_SupplierId",
                table: "SupplierBranches");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_AspNetUsers_UserId",
                table: "Suppliers");

            migrationBuilder.DropTable(
                name: "Auctions");

            migrationBuilder.DropTable(
                name: "PaymentTransfers");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_UserId",
                table: "Suppliers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseRequests",
                table: "PurchaseRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bids",
                table: "Bids");

            migrationBuilder.DropIndex(
                name: "IX_Bids_AuctionId",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
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
                name: "PaymentMethod2",
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

            migrationBuilder.DropColumn(
                name: "BudgetComment",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "ReviewedComment",
                table: "PurchaseRequests");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "GoodsReceived");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "GoodsReceived");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Bids");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Suppliers",
                newName: "SupplierID");

            migrationBuilder.RenameColumn(
                name: "ExistingItemId",
                table: "PurchaseRequests",
                newName: "reviwed_by");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PurchaseRequests",
                newName: "CreatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseRequests_ExistingItemId",
                table: "PurchaseRequests",
                newName: "IX_PurchaseRequests_reviwed_by");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Items",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Bids",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "AuctionId",
                table: "Bids",
                newName: "BidId");

            migrationBuilder.AddColumn<string>(
                name: "purchaseRequestId",
                table: "PurchaseRequests",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Item",
                table: "PurchaseRequests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ItemDescription",
                table: "PurchaseRequests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReviewdComment",
                table: "PurchaseRequests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Unit",
                table: "PurchaseRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "existingItem",
                table: "PurchaseRequests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "existingItems",
                table: "PurchaseRequests",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "remark",
                table: "PurchaseRequests",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Items",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<string>(
                name: "ItemId",
                table: "Items",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Items",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CurrentStock",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Items",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UOM",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "Items",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Items",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "GoodsReceived",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "GoodsReceived",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "UOM",
                table: "GoodsReceived",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Bids",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupplierId",
                table: "AspNetUsers",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers",
                column: "SupplierID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseRequests",
                table: "PurchaseRequests",
                column: "purchaseRequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "ItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bids",
                table: "Bids",
                column: "BidId");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "15bb9e8f-9934-4ec4-9f30-e5e0a9ffe5ea", null, "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "AccountStatus", "Address", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "Gender", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "SupplierId", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d8a17e37-3489-4a0c-a577-98a185befbda", 0, 0, "123 Admin Street", "3b073236-3fb0-49f7-beba-2b494fc26af9", "admin@gmail.com", true, "Admin", 0, "User", false, null, "ADMIN@GMAIL.COM", "ADMIN", "AQAAAAIAAYagAAAAEHEYqipMJpCO1m4FQaRN1KPQ2dKFu3blJPIdWUFmGTFzNhce/P6gFwWrHTo/IbgEgw==", null, false, "e78697b2-6fed-49c7-8bb2-c0c70b590d9d", null, false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "15bb9e8f-9934-4ec4-9f30-e5e0a9ffe5ea", "d8a17e37-3489-4a0c-a577-98a185befbda" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_CreatedBy",
                table: "PurchaseRequests",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_existingItems",
                table: "PurchaseRequests",
                column: "existingItems");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryId",
                table: "Items",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SupplierId",
                table: "AspNetUsers",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Suppliers_SupplierId",
                table: "AspNetUsers",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierID");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Items_ItemId",
                table: "Bids",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_PurchaseRequests_PurchaseRequestId",
                table: "Bids",
                column: "PurchaseRequestId",
                principalTable: "PurchaseRequests",
                principalColumn: "purchaseRequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Suppliers_SupplierId",
                table: "Bids",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceived_Bids_BidId",
                table: "GoodsReceived",
                column: "BidId",
                principalTable: "Bids",
                principalColumn: "BidId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceived_Items_ItemId",
                table: "GoodsReceived",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceived_PurchaseRequests_PurchaseRequestId",
                table: "GoodsReceived",
                column: "PurchaseRequestId",
                principalTable: "PurchaseRequests",
                principalColumn: "purchaseRequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_CategoryId",
                table: "Items",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_AspNetUsers_CreatedBy",
                table: "PurchaseRequests",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_AspNetUsers_reviwed_by",
                table: "PurchaseRequests",
                column: "reviwed_by",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseRequests_Items_existingItems",
                table: "PurchaseRequests",
                column: "existingItems",
                principalTable: "Items",
                principalColumn: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierBranches_Suppliers_SupplierId",
                table: "SupplierBranches",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
