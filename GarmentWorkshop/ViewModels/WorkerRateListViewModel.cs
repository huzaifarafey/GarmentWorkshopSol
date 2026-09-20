using GarmentWorkshop.Models;

namespace GarmentWorkshop.ViewModels
{
    public class WorkerRateListViewModel
    {
        public List<WorkerRate> CurrentRates { get; set; } = new();
        public List<WorkerRate> UpcomingRates { get; set; } = new();
        public List<WorkerRate> HistoryRates { get; set; } = new();
    }
}