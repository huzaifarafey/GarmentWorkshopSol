using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarmentWorkshop.Models
{
    public enum ThreadTransactionType
    {
        Purchase = 1,
        Consumption = 2
    }

    public class ThreadTransaction
    {
        public int Id { get; set; }

        [Required]
        public int ThreadStockId { get; set; }
        public ThreadStock? ThreadStock { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public ThreadTransactionType Type { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Quantity { get; set; }
    }
}