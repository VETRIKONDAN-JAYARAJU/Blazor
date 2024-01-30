using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class Deposit
    {
        [Key]
        public int DepositId { get; set; }

        [Required]
        public int MerchantId { get; set; }

        public string? MerchantName { get; set; }

        public string? OrderCorrespondenceId { get; set; }

        [Required]
        public string? MemberCorrespondenceId { get; set; }

        public string? ReferenceNo { get; set; } 

        [Required]
        public int PaymentTypeId { get; set; }

        public string? PaymentType { get; set; }

        [Required]
        public int BankId { get; set; }

        public string? BankName { get; set; }

        [Required]
        public int BankAccountId { get; set; }

        public string? AccountNo { get; set; }

        public string? DeviceName { get; set; }

        [Required]
        public string CurrencyCode { get; set; } = string.Empty;

        [Required]
        public decimal? Amount { get; set; }

        public decimal? ActualAmount { get; set; }

        public decimal? BalanceAmount { get; set; }

        public decimal? Credit { get; set; } = 0;

        public decimal? Debit { get; set; } = 0;

        public int? EWalletId { get; set; } 

        public string? EWalletName { get; set; }

        public int? QRCodeId { get; set; } 

        public string? QRCodeName { get; set; }

        public string? TransactionCode { get; set; }

        public int StatusId { get; set; }

        public string? Status { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;

    }




}
