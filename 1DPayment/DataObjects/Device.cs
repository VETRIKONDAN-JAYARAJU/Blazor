using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class Device
    {
        [Key]
        public int DeviceId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string? DeviceName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string? UUID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string? PUID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 10)]
        public string? DeviceKey { get; set; }

        [Required]
        public string? DeviceType { get; set; }

        public int StatusId { get; set; } 

        public string? Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
