namespace WorkshopFlow.DTO
{
    public record RoleReadOnlyDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}

