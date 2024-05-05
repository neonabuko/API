using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScoreHubAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class migration_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "File",
                table: "Songs");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "Songs",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Songs",
                type: "nvarchar(70)",
                maxLength: 70,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Songs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Songs");

            migrationBuilder.AddColumn<byte[]>(
                name: "File",
                table: "Songs",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: Array.Empty<byte>());
        }
    }
}
