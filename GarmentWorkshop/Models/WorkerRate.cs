using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarmentWorkshop.Models
{
    public class WorkerRate
    {
        public int Id { get; set; }

        [Required]
        public int WorkerId { get; set; }
        public Worker? Worker { get; set; } 

        [Required]
        public int GarmentId { get; set; }
        public Garment? Garment { get; set; } 

        [Column(TypeName = "decimal(10,2)")]
        public decimal RatePerPiece { get; set; }

        [Required]
        public DateTime EffectiveFrom { get; set; } // Effective date means the date from which this rate is applicable
    }
}