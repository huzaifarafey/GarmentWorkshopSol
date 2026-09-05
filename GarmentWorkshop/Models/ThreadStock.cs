using System.ComponentModel.DataAnnotations;

namespace GarmentWorkshop.Models
{
    public class ThreadStock
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? ColorOrType { get; set; }   // e.g. "White", "Black 40s"

        public decimal CurrentQuantity { get; set; }   // running balance, in your unit (e.g. cones/meters)

        public ICollection<ThreadTransaction> Transactions { get; set; } = new List<ThreadTransaction>();
    }
}