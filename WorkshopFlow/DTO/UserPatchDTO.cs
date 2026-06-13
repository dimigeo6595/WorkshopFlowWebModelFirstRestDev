using System.ComponentModel.DataAnnotations;

namespace WorkshopFlow.DTO
{
    public record UserPatchDTO
    {
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        public string? CurrentPassword { get; set; }

        [RegularExpression(@"(?=.*?[A-Z])(?=.*?[a-z])(?=.*?\d)(?=.*?\W)^.{8,}$",
            ErrorMessage = "Password must contain at least 8 characters, one uppercase, one lowercase, one digit and one special character.")]
        public string? NewPassword { get; set; }
    }
}