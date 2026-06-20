using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkshopFlow.Migrations
{
    /// <inheritdoc />
    public partial class FilterBomLineUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_BomLines_ProducedItem_ComponentItem",
                table: "BomLines");

            migrationBuilder.CreateIndex(
                name: "UQ_BomLines_ProducedItem_ComponentItem",
                table: "BomLines",
                columns: new[] { "ProducedItemId", "ComponentItemId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_BomLines_ProducedItem_ComponentItem",
                table: "BomLines");

            migrationBuilder.CreateIndex(
                name: "UQ_BomLines_ProducedItem_ComponentItem",
                table: "BomLines",
                columns: new[] { "ProducedItemId", "ComponentItemId" },
                unique: true);
        }
    }
}
