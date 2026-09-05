using GarmentWorkshop.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GarmentWorkshop.ViewModels
{
    public class DailyEntryViewModel
    {
        // Form inputs
        public Production ProductionEntry { get; set; } = new() { Date = DateTime.Today };
        public Expense ExpenseEntry { get; set; } = new() { Date = DateTime.Today };
        public MachineMaintenance MaintenanceEntry { get; set; } = new() { Date = DateTime.Today };
        public ThreadTransaction ThreadEntry { get; set; } = new() { Date = DateTime.Today };

        // Dropdown sources
        public SelectList? Workers { get; set; }
        public SelectList? WorkOrders { get; set; }
        public SelectList? ExpenseCategories { get; set; }
        public SelectList? Machines { get; set; }
        public SelectList? ThreadStocks { get; set; }

        // Today's activity, shown at bottom
        public List<RecentEntry> TodayEntries { get; set; } = new();
    }

    public class RecentEntry
    {
        public string? Type { get; set; }        // "Production", "Expense", "Maintenance", "Thread"
        public string? Description { get; set; }
        public string? Value { get; set; }
    }
}