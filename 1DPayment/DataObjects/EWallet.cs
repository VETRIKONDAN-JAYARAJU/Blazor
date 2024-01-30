using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class EWallet
    {
        [Key]
        public int EWalletId { get; set; }

        [Required]
        public string? EWalletNo { get; set; }

        [Required]
        public string? EWalletName { get; set; }

        [Required]
        public int QRCodeId { get; set; }

        public string? QRCodeName { get; set; }

        public int StatusId { get; set; }

        public string? Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
