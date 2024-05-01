using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SongManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class migration_9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Songs_Name",
                table: "Songs",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scores_Name",
                table: "Scores",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Songs_Name",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Scores_Name",
                table: "Scores");
        }
    }
}
