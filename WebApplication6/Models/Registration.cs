using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class Registration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "ID Should Not Be Null")]
        public string ID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MinLength(4, ErrorMessage = "Name should contain a minimum length of 4 characters")]
        [MaxLength(30, ErrorMessage = "Name should not exceed the maximum limit of 30 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required.")]
        [Range(1000000000, 9999999999, ErrorMessage = "Phone number should contain exactly 10 digits.")]
        public long PhoneNumber { get; set; }

        [Required]
        public DateOnly D_O_B { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$%^&*])(?=.*\d).+$", ErrorMessage = "Password must contain at" +
            " least one uppercase letter, one special character, and one number.")]
        public string Password { get; set; }

        [Required]
        public string Answer { get; set; }
        public string Signature { get; set; }
        [Required]
        public string RoleofUser { get; set; }
    }
}
