namespace WorkshopFlow.DTO
{
    public record WorkstationReadOnlyDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Notes { get; set; }
    }
}
