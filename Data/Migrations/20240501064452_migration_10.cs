using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScoreHubAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class migration_10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bitrate",
                table: "Songs");

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Songs",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<int>(
                name: "Bitrate",
                table: "Songs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
