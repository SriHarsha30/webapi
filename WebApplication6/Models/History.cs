using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models

{
    public class History
    {
        
        public string Tenant_id { get; set; }   

        [Required]
        public string Tenant_name { get; set; }

        [Required]
        public long Tenant_Phonenumber { get; set; }

        [Required]
        public int leased_property_id { get ; set; }

        [Required]
        public DateTime startTime { get; set; }

        [Required]
        public DateTime endTime { get; set; }


    }
}
