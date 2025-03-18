using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class Payment

    {

        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        [Range(100000, 999999, ErrorMessage = "PaymentID must be a 6-digit number.")]

        public int PaymentID { get; set; }

        [Required]

        public string Tenant_Id { get; set; }

        [Required]

        public int PropertyId { get; set; }

        [Required]

        [Range(0.1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]

        public decimal Amount { get; set; }

        [Required]

        public DateTime PaymentDate { get; set; }

        [Required]

        [StringLength(50, ErrorMessage = "Status cannot be longer than 50 characters.")]

        public string Status { get; set; }

        [StringLength(50, ErrorMessage = "Ownerstatus cannot be longer than 50 characters.")]

        public string Ownerstatus { get; set; }

        public Payment()

        {

            Random random = new Random();

            this.PaymentID = random.Next(100000, 1000000);

            this.PaymentDate = DateTime.Now;

        }


    }

}
