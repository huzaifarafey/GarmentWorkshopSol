using GarmentWorkshop.Models;

namespace GarmentWorkshop.ViewModels
{
    public class ProductionRow
    {
        public Production Production { get; set; }
        public decimal Earning { get; set; }
    }

    public class ProductionListViewModel
    {
        public List<ProductionRow> TodayEntries { get; set; } = new();
        public List<ProductionRow> ThisWeekEntries { get; set; } = new();
        public List<ProductionRow> EarlierEntries { get; set; } = new();
    }
}