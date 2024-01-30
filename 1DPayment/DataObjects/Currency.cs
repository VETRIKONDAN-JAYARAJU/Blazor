using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class Currency
    {
        [Key]
        public int CurrencyId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? CurrencyName { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public string? CurrencyCode { get; set; }

        public int StatusId { get; set; }

        public string? Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
