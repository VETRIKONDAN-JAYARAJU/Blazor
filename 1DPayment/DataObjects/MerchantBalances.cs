using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class MerchantBalances
    {
        [Key]
        public int MerchantBalanceId { get; set; }

        public int MerchantId { get; set; }

        public string MerchantName { get; set; } = string.Empty;

        public string CurrencyCode { get; set; } = string.Empty;

        public decimal BalanceAmount { get; set; }

    }
}
