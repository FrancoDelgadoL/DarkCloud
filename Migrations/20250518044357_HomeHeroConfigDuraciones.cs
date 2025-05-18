using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkCloud.Migrations
{
    /// <inheritdoc />
    public partial class HomeHeroConfigDuraciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DuracionesJson",
                table: "HomeHeroConfigs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuracionesJson",
                table: "HomeHeroConfigs");
        }
    }
}
