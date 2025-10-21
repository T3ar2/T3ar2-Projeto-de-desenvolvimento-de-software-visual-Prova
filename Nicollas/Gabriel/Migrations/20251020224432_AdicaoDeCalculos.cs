using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gabriel.Migrations
{
    /// <inheritdoc />
    public partial class AdicaoDeCalculos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AdicionalBandeira",
                table: "ConsumoDeAguas",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Tarifa",
                table: "ConsumoDeAguas",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TaxaEsgoto",
                table: "ConsumoDeAguas",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Total",
                table: "ConsumoDeAguas",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ValorAgua",
                table: "ConsumoDeAguas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdicionalBandeira",
                table: "ConsumoDeAguas");

            migrationBuilder.DropColumn(
                name: "Tarifa",
                table: "ConsumoDeAguas");

            migrationBuilder.DropColumn(
                name: "TaxaEsgoto",
                table: "ConsumoDeAguas");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "ConsumoDeAguas");

            migrationBuilder.DropColumn(
                name: "ValorAgua",
                table: "ConsumoDeAguas");
        }
    }
}
