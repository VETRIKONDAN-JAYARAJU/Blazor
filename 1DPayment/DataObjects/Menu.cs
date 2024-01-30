using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class Menu
    {
        [Key]
        public int MenuId { get; set; }
                
        public string? MenuName { get; set; }

        public string? PageName { get; set; }

        public string? IconName  { get; set; }

        public bool MenuStatus { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
