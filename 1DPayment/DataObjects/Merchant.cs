using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class Merchant
    {
        [Key]
        public int MerchantId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string? MerchantName { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string? ApiUrl { get; set; }

        [Required]
        public int BankId { get; set; }

        public string? BankName { get; set; }

        [Required]
        public int DeviceId { get; set; } 

        public string? DeviceKey { get; set; }

        public int UpperMerchant { get; set; }

        public string? UpperMerchantName { get; set; }

        [Required]
        public string? MerchantType { get; set; }

        [Required]
        public decimal? Commission { get; set; }

        [Required]
        public string? OperationMode { get; set; }

        public int StatusId { get; set; }

        public string? Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
