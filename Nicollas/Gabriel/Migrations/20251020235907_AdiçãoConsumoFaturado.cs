using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gabriel.Migrations
{
    /// <inheritdoc />
    public partial class AdiçãoConsumoFaturado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ConsumoFaturado",
                table: "ConsumoDeAguas",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsumoFaturado",
                table: "ConsumoDeAguas");
        }
    }
}
