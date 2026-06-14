namespace WorkshopFlow.DTO
{
    public record WorkOrderOperationReadOnlyDTO
    {
        public int Id { get; set; }
        public int Sequence { get; set; }
        public string Status { get; set; } = null!;
        public string OperationName { get; set; } = null!;
        public string WorkstationCode { get; set; } = null!;
        public string WorkstationName { get; set; } = null!;
        public string? MachineCode { get; set; }
        public string? MachineName { get; set; }
        public string? AssignedToUsername { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string? Notes { get; set; }
    }
}
