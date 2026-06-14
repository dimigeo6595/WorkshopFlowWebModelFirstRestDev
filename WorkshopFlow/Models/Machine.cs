namespace WorkshopFlow.Models
{
    public class Machine : BaseEntity
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Notes { get; set; }

        // FK
        public int WorkstationId { get; set; }

        // Navigation properties
        public Workstation Workstation { get; set; } = null!;
        public ICollection<RoutingStep> RoutingSteps { get; set; } = new HashSet<RoutingStep>();
    }
}
