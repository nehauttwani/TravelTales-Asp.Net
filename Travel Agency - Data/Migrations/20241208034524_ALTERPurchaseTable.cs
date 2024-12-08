using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travel_Agency___Data.Migrations
{
    /// <inheritdoc />
    public partial class ALTERPurchaseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetails_Products_Suppliers",
                table: "BookingDetails");

            migrationBuilder.DropIndex(
                name: "Products_SuppliersBookingDetails",
                table: "BookingDetails");

            migrationBuilder.DropIndex(
                name: "ProductSupplierId",
                table: "BookingDetails");

            migrationBuilder.DropColumn(
                name: "ProductSupplierId",
                table: "BookingDetails");

            //migrationBuilder.AlterColumn<string>(
            //    name: "ImagePath",
            //    table: "Packages",
            //    type: "nvarchar(255)",
            //    maxLength: 255,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    PurchaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.PurchaseId);
                    table.ForeignKey(
                        name: "FK_Purchases_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_CustomerId",
                table: "Purchases",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Purchases");

            //migrationBuilder.AlterColumn<string>(
            //    name: "ImagePath",
            //    table: "Packages",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "",
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(255)",
            //    oldMaxLength: 255,
            //    oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductSupplierId",
                table: "BookingDetails",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "Products_SuppliersBookingDetails",
                table: "BookingDetails",
                column: "ProductSupplierId");

            migrationBuilder.CreateIndex(
                name: "ProductSupplierId",
                table: "BookingDetails",
                column: "ProductSupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetails_Products_Suppliers",
                table: "BookingDetails",
                column: "ProductSupplierId",
                principalTable: "Products_Suppliers",
                principalColumn: "ProductSupplierId");
        }
    }
}
