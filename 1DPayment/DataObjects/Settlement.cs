using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class Settlement
    {
        [Key]
        public int SettlementId { get; set; }

        [Required]
        public int MerchantId { get; set; }

        public string? MerchantName { get; set; }

        [Required]
        public int BankId { get; set; }

        public string? BankName { get; set; }

        [Required]
        public int BankAccountId { get; set; }

        public string? AccountNo { get; set; }

        public string? Name { get; set; }

        public string? NickName { get; set; }

        [Required]
        public int SourceMerchantId { get; set; }

        public string? SourceMerchantName { get; set; }

        [Required]
        public int SourceBankId { get; set; }

        public string? SourceBankName { get; set; }

        [Required]
        public int SourceBankAccountId { get; set; }

        public string? SourceAccountNo { get; set; }

        [Required]
        public string? CurrencyCode { get; set; }

        [Required]
        public decimal? Amount { get; set; }

        public string? Remarks { get; set; }

        public string? ReferenceNo { get; set; }

        public string? TransactionCode { get; set; }

        public int StatusId { get; set; }

        public string? Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;

    }
}
