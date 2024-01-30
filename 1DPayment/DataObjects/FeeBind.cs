using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class FeeBind
    {
        [Key]
        public int FeeBindId { get; set; }

        [Required]
        public int MerchantId { get; set; }

        public string? MerchantName { get; set; } = string.Empty;

        [Required]
        public int PaymentTypeId { get; set; }

        public string? PaymentType { get; set; } = string.Empty;

        [Required]
        public string? CurrencyCode { get; set; } = string.Empty;

        [Required]
        public decimal? FeeRate { get; set; }

        [Required]
        public string? FeeBindType { get; set; } = string.Empty;

        [Required]
        public decimal? MinAmount { get; set; }

        [Required]
        public decimal? MaxAmount { get; set; }

        [Required]
        public decimal? LimitAmount { get; set; }

        [Required]
        public int SourceMerchant { get; set; }

        public string? SourceMerchantName { get; set; } = string.Empty;

        public int StatusId { get; set; }

        public string? Status { get; set; } = string.Empty;

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
