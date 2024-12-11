using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travel_Agency___Data.Migrations
{
    /// <inheritdoc />
    public partial class picture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if the column exists first
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true); // You can adjust to your requirement if nullable or not

            // Now alter the column if needed (e.g., change type, set default value, etc.)
            migrationBuilder.AlterColumn<string>(
                name: "ProfilePicture",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);
        }
    }
}
