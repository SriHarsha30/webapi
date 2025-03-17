using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication6.Models
{
    public enum Status
    {
        Pending,
        Accepted,
        Rejected
    }

    public class Maintainance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Property ID is required.")]
        public int PropertyId { get; set; }

        [Required(ErrorMessage = "Tenant ID is required.")]
        public string TenantId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? Description { get; set; }

        [Required]
        [EnumDataType(typeof(Status), ErrorMessage = "Status must be either 'Pending', 'Accepted', or 'Rejected'.")]
        public String Status { get; set; }

        public string? ImagePath { get; set; }
        
    }
}