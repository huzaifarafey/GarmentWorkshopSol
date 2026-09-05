using System.ComponentModel.DataAnnotations;

namespace GarmentWorkshop.Models;

public enum PartyStatus
{
    Active = 1,
    Inactive = 2
}

public class Party
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    [Required]
    [MaxLength(15)]
    public string Mobile { get; set; }

    public PartyStatus Status { get; set; } = PartyStatus.Active;

    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
}