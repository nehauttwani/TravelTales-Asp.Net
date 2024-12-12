using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travel_Agency___Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Packages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Packages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
