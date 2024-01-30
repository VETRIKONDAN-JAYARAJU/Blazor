using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class Bank
    {
        [Key]
        public int BankId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string? BankName { get; set; }

        public int StatusId { get; set; }

        public string? Status { get; set; }

        public int CreatedBy { get; set; } 

        public int UpdatedBy { get; set; } 

        public DateTime UpdatedOn { get; set; } = DateTime.Now; 
    }
}