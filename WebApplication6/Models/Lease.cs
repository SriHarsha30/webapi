using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class Lease
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeaseId { get; set; }

        [Required]
        public string? ID { get; set; }

        [ForeignKey("ID")]
        public virtual Registration? Tenant { get; set; }

        [Required]
        public int? Property_Id { get; set; }

        [ForeignKey("Property_Id")]
        public virtual Property? Prop { get; set; }

        [Required]
        [DataType(DataType.Date, ErrorMessage = "wrong format")]
        public DateTime? StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required]
        public bool? Tenant_Signature { get; set; }

        [Required]
        public bool? Owner_Signature { get; set; }

        [Required]
        public bool? Lease_status { get; set; }
    }
}
