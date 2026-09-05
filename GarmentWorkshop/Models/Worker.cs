using System.ComponentModel.DataAnnotations;

namespace GarmentWorkshop.Models
{
    public enum WorkerStatus
    {
        Active = 1,
        Inactive = 2
    }

    public class Worker
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(15)]
        public string Phone { get; set; }

        public WorkerStatus Status { get; set; } = WorkerStatus.Active;

        public ICollection<WorkerRate> WorkerRates { get; set; } = new List<WorkerRate>();
        public ICollection<Production> Productions { get; set; } = new List<Production>();
    }
}