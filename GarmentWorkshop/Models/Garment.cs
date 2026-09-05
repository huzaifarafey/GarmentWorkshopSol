using System.ComponentModel.DataAnnotations;

namespace GarmentWorkshop.Models;

public enum GarmentStatus
{
    Active = 1,
    Inactive = 2
}

public class Garment
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; }   // optional, e.g. "Tops" — can be null

    public GarmentStatus Status { get; set; } = GarmentStatus.Active;

    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    public ICollection<WorkerRate> WorkerRates { get; set; } = new List<WorkerRate>();
}