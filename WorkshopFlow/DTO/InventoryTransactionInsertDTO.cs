using System.ComponentModel.DataAnnotations;
using WorkshopFlow.Models;
using WorkshopFlow.Models.Enums;

namespace WorkshopFlow.DTO
{
    public record InventoryTransactionInsertDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        public int? ItemId { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public decimal? Quantity { get; set; }

        // Επιχειρησιακός κανόνας: μόνο Purchase και Adjustment επιτρέπονται manual
        // Production και Consumption δημιουργούνται αυτόματα από WorkOrder
        [Required(ErrorMessage = "The {0} field is required.")]
        public TransactionType? TransactionType { get; set; }

        [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
