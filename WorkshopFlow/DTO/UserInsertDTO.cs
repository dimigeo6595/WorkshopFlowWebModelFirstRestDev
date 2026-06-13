using System.ComponentModel.DataAnnotations;

namespace WorkshopFlow.DTO
{
    public record UserInsertDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?\W)^.{8,}$",
            ErrorMessage = "Password must contain at least 8 characters, one uppercase, one lowercase, one digit and one special character.")]
        public string? Password { get; set; }

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