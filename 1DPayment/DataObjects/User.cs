using System.ComponentModel.DataAnnotations;

namespace DataObjects
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string? Name { get; set; }

        [Required(ErrorMessage = "Email ID is Required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [MaxLength(50)]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage = "Please Enter Valid Email")]

        public string Email { get; set; } =  "justin@gmail.com";  // string.Empty; //

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; } =  "Justin@1"; // string.Empty; //

        public int RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public int StatusId { get; set; }

        public string? Status { get; set; }

        public int CreatedBy { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
