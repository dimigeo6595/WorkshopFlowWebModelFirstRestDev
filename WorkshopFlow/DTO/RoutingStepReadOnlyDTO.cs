namespace WorkshopFlow.DTO
{
    public record RoutingStepReadOnlyDTO
    {
        public int Id { get; set; }
        public int Sequence { get; set; }
        public string OperationName { get; set; } = null!;
        public int EstimatedMinutes { get; set; }
        public string WorkstationCode { get; set; } = null!;
        public string WorkstationName { get; set; } = null!;
        public string? MachineCode { get; set; }
        public string? MachineName { get; set; }
        public string? Notes { get; set; }
    }
}
