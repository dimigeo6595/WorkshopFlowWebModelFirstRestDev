using WorkshopFlow.Models;

namespace WorkshopFlow.Models;

public class User : BaseEntity
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;

    // Navigation properties
    public ICollection<WorkOrder> CreatedWorkOrders { get; set; } = new HashSet<WorkOrder>();
    public ICollection<WorkOrderOperation> AssignedOperations { get; set; } = new HashSet<WorkOrderOperation>();
    public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new HashSet<InventoryTransaction>();

}
