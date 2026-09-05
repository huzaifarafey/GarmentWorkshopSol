using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarmentWorkshop.Models
{
    public enum WorkOrderStatus
    {
        Open = 1,
        Completed = 2
    }

    public class WorkOrder
    {
        public int Id { get; set; }

        [Required]
        public int PartyId { get; set; }
        public Party? Party { get; set; }

        [Required]
        public int GarmentId { get; set; }
        public Garment? Garment { get; set; }

        [Required]
        public int TotalPieces { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PartyRatePerPiece { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? ExpectedEndDate { get; set; }

        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Open;

        public ICollection<Production> Productions { get; set; } = new List<Production>();
    }
}