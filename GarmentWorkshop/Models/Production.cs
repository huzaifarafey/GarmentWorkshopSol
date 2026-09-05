using System.ComponentModel.DataAnnotations;

namespace GarmentWorkshop.Models
{
    public class Production
    {
        public int Id { get; set; }

        [Required]
        public int WorkerId { get; set; }
        public Worker? Worker { get; set; }

        [Required]
        public int WorkOrderId { get; set; }
        public WorkOrder? WorkOrder { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int PiecesProduced { get; set; }
    }
}
