using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class Payout
    {
        [Key]
        public int PayoutId { get; set; }

        [Required]
        public int MerchantId { get; set; }

        public string? MerchantName { get; set; }

        [Required]
        public int BankId { get; set; }

        public string? BankName { get; set; }

        [Required]
        public int BankAccountId { get; set; }

        public string? AccountNo { get; set; }

        public string? DeviceName { get; set; }

        public string? OrderCorrespondenceId { get; set; }

        [Required]
        public string? MemberCorrespondenceId { get; set; }

        public string? ReferenceNo { get; set; }

        [Required]
        public int PaymentTypeId { get; set; }

        public string? PaymentType { get; set; }

        [Required]
        public string? CurrencyCode { get; set; }

        [Required]
        public string? TargetAccountNo { get; set; }

        [Required]
        public string? CustomerName { get; set; }

        [Required]
        public decimal? Amount { get; set; }

        public decimal? ActualAmount { get; set; }

        public string? TransactionCode { get; set; }

        public int StatusId { get; set; }

        public string? Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}