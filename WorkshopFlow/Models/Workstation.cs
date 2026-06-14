using System.Reflection.PortableExecutable;

namespace WorkshopFlow.Models
{
    public class Workstation : BaseEntity
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Notes { get; set; }

        // Navigation properties
        public ICollection<Machine> Machines { get; set; } = new HashSet<Machine>();
        public ICollection<RoutingStep> RoutingSteps { get; set; } = new HashSet<RoutingStep>();
    }
}
