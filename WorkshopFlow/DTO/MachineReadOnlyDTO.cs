namespace WorkshopFlow.DTO
{
    public record MachineReadOnlyDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Notes { get; set; }
        public string WorkstationCode { get; set; } = null!;
        public string WorkstationName { get; set; } = null!;
    }
}
