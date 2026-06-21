using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkshopFlow.Migrations
{
    /// <inheritdoc />
    public partial class FilterAllSoftDeleteUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_Workstations_Code",
                table: "Workstations");

            migrationBuilder.DropIndex(
                name: "UQ_WorkOrders_WorkOrderCode",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "UQ_WorkOrderOperations_WorkOrder_Sequence",
                table: "WorkOrderOperations");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "UQ_Machines_Code",
                table: "Machines");

            migrationBuilder.DropIndex(
                name: "UQ_Items_ItemCode",
                table: "Items");

            migrationBuilder.CreateIndex(
                name: "UQ_Workstations_Code",
                table: "Workstations",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UQ_WorkOrders_WorkOrderCode",
                table: "WorkOrders",
                column: "WorkOrderCode",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UQ_WorkOrderOperations_WorkOrder_Sequence",
                table: "WorkOrderOperations",
                columns: new[] { "WorkOrderId", "Sequence" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UQ_Machines_Code",
                table: "Machines",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "UQ_Items_ItemCode",
                table: "Items",
                column: "ItemCode",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_Workstations_Code",
                table: "Workstations");

            migrationBuilder.DropIndex(
                name: "UQ_WorkOrders_WorkOrderCode",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "UQ_WorkOrderOperations_WorkOrder_Sequence",
                table: "WorkOrderOperations");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "UQ_Machines_Code",
                table: "Machines");

            migrationBuilder.DropIndex(
                name: "UQ_Items_ItemCode",
                table: "Items");

            migrationBuilder.CreateIndex(
                name: "UQ_Workstations_Code",
                table: "Workstations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_WorkOrders_WorkOrderCode",
                table: "WorkOrders",
                column: "WorkOrderCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_WorkOrderOperations_WorkOrder_Sequence",
                table: "WorkOrderOperations",
                columns: new[] { "WorkOrderId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Machines_Code",
                table: "Machines",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Items_ItemCode",
                table: "Items",
                column: "ItemCode",
                unique: true);
        }
    }
}
