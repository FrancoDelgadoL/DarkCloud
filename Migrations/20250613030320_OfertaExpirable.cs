using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkCloud.Migrations
{
    /// <inheritdoc />
    public partial class OfertaExpirable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuracionOfertaHoras",
                table: "Productos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OfertaFin",
                table: "Productos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OfertaInicio",
                table: "Productos",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuracionOfertaHoras",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "OfertaFin",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "OfertaInicio",
                table: "Productos");
        }
    }
}
