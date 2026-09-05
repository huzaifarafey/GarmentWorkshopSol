using System.ComponentModel.DataAnnotations;

namespace GarmentWorkshop.Models
{
    public enum MachineType
    {
        Sewing = 1,
        Warlock = 2
    }

    public enum MachineStatus
    {
        Active = 1,
        Inactive = 2
    }

    public class Machine
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }   // e.g. "Juki 1", "Warlock 1"

        public MachineType Type { get; set; }

        public MachineStatus Status { get; set; } = MachineStatus.Active;

        public ICollection<MachineMaintenance> MaintenanceRecords { get; set; } = new List<MachineMaintenance>();
    }
}