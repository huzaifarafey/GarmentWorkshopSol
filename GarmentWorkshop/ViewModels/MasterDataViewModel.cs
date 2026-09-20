using GarmentWorkshop.Models;

namespace GarmentWorkshop.ViewModels
{
    public class MasterDataViewModel
    {
        public List<Worker> Workers { get; set; } = new();
        public List<Party> Parties { get; set; } = new();
        public List<Machine> Machines { get; set; } = new();
    }
}