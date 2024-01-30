using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class UserMenu
    {
        [Key]
        public int UserMenuId { get; set; }

        public int RoleId { get; set; }

        public string? RoleName { get; set; }

        public string MenuIds { get; set; } = string.Empty;

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
