namespace WorkshopFlow.DTO
{
    public record UnitOfMeasureReadOnlyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Symbol { get; set; } = null!;
    }
}
