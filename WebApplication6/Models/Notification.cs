using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Notification_Id { get; set; }

        [Required]
        public string? sendersId { get; set; }

        private string? _receiversId;

        [Required]
        public string? receiversId
        {
            get => _receiversId ?? sendersId;
            set => _receiversId = value;
        }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public string? notification_Descpirtion { get; set; }
    }
}
