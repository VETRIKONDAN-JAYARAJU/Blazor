using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class BankAccount
    {
        [Key]
        public int BankAccountId { get; set; }

        [Required]
        public int BankId { get; set; }

        public string? BankName { get; set; }

        [Required]
        public string? AccountNo { get; set; }


        [Required]
        public string? Name { get; set; }

        [Required]
        public string? NickName { get; set; }

        [Required]
        public int QRCodeId { get; set; }

        public string? QRCodeName { get; set; }

        [Required]
        [Url]
        public string? BankLoginUrl { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? LoginId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 8)]
        public string? Password { get; set; }

        [Required]
        public string? Province { get; set; }

        [Required]
        public string? State { get; set; }

        [Required]
        public string? Type { get; set; }

        [Required]
       // [RegularExpression(@"^(\d*\.)?\d+$", ErrorMessage = "Please Enter Valid Amount")]
        [RegularExpression(@"^[0-9]*(?:\.[0-9]{0,4})?$", ErrorMessage = "Please Enter Valid Amount")]

        public decimal? BalanceAmount { get; set; }

        [Required]
        public string? CurrencyCode { get; set; }

        public bool BotStatus { get; set; }

        public int StatusId { get; set; }

        public string? Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
