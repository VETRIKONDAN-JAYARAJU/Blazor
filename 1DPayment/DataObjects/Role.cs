using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public string? RoleName { get; set; }

        public int StatusId { get; set; }

        public string? Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
