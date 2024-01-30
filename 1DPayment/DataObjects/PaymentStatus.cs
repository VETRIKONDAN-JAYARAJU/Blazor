using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class PaymentStatus
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string? Status { get; set; }

        public int CreatedBy { get; set; }
    }
}
