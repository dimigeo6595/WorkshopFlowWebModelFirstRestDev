using System.ComponentModel.DataAnnotations;

namespace WorkshopFlow.DTO
{
    public record UserUpdateDTO
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Firstname { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Lastname { get; set; }

        [Required]
        public int? RoleId { get; set; }
    }
}