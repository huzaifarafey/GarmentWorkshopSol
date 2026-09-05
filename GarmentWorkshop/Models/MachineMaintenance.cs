using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarmentWorkshop.Models
{
    public class MachineMaintenance
    {
        public int Id { get; set; }

        [Required]
        public int MachineId { get; set; }
        public Machine? Machine { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [MaxLength(200)]
        public string? Note { get; set; }   // e.g. "Needle repair", "Service" — optional
    }
}