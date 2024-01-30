using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class QRCode
    {
        [Key]
        public int QRCodeId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? QRCodeName { get; set; }

        [Required(ErrorMessage = "Upload only (.png/.jpg) format and file size below 1MB")]
        public string? FileName { get; set; }

        public string? FilePath { get; set; }

        public int StatusId { get; set; }

        public string? Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;

    }
}
