using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkCloud.Migrations
{
    /// <inheritdoc />
    public partial class AddImagenPrincipalToProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImagenPrincipal",
                table: "Productos",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenPrincipalMimeType",
                table: "Productos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenPrincipal",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ImagenPrincipalMimeType",
                table: "Productos");
        }
    }
}
