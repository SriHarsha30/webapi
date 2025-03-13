using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class Property
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Property_Id { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool AvailableStatus { get; set; }

        [ForeignKey("Registration")]
        public string Owner_Id { get; set; }

        // Properties to store owner details
        //public Registration? Registration { get; set; }
        [NotMapped]
        public string Owner_Name { get; set; }

        [NotMapped]
        public long? Owner_PhoneNumber { get; set; }
    }
}
