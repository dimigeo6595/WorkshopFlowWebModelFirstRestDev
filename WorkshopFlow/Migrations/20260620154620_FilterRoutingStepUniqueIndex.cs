using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkshopFlow.Migrations
{
    /// <inheritdoc />
    public partial class FilterRoutingStepUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoutingSteps_ProducedItemId",
                table: "RoutingSteps");

            migrationBuilder.DropIndex(
                name: "UQ_RoutingSteps_ProducedItem_Sequence",
                table: "RoutingSteps");

            migrationBuilder.CreateIndex(
                name: "UQ_RoutingSteps_ProducedItem_Sequence",
                table: "RoutingSteps",
                columns: new[] { "ProducedItemId", "Sequence" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_RoutingSteps_ProducedItem_Sequence",
                table: "RoutingSteps");

            migrationBuilder.CreateIndex(
                name: "IX_RoutingSteps_ProducedItemId",
                table: "RoutingSteps",
                column: "ProducedItemId");

            migrationBuilder.CreateIndex(
                name: "UQ_RoutingSteps_ProducedItem_Sequence",
                table: "RoutingSteps",
                columns: new[] { "ProducedItemId", "Sequence" },
                unique: true);
        }
    }
}
