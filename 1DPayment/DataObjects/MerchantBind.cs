using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class MerchantBind
    {
        [Key]
        public int MerchantBindId { get; set; }

        [Required]
        public int MerchantId { get; set; }

        public string? MerchantName { get; set; }

        [Required]
        public int BankId { get; set; }

        public string? BankName { get; set; }

        [Required]
        public int BankAccountId { get; set; }

        public string? AccountNo { get; set; }

        public int StatusId { get; set; }

        public string? Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}