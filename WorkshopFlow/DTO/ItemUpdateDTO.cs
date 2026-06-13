using System.ComponentModel.DataAnnotations;
using WorkshopFlow.Models;

namespace WorkshopFlow.DTO
{
    public record ItemUpdateDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public ItemType? ItemType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "WeightPerUoM must be a positive number.")]
        public decimal? WeightPerUoM { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public int? UnitOfMeasureId { get; set; }

        public int? WeightUoMId { get; set; }
    }
}