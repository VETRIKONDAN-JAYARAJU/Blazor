using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class Commission
    {
        [Key]
        public int CommissionId { get; set; }

        public int MerchantId { get; set; } // MerchantId from Deposit Table

        public string MerchantName { get; set; } = string.Empty;

        public int TransactionId { get; set; } // DepositId from Deposit Table

        public decimal TransactionValue { get; set; }

        public decimal CommissionEarn { get; set; }

        public decimal NewBalance { get; set; }

        public string CurrencyCode { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; } // Createdon from Deposit Table

        public DateTime CommissionDate { get; set; } 
    }
}
