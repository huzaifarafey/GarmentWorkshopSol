namespace GarmentWorkshop.ViewModels
{
    public class DashboardViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? PeriodLabel { get; set; }

        // Production
        public int TotalPiecesProduced { get; set; }
        public List<WorkerProductionSummary> WorkerWiseProduction { get; set; } = new();

        // Worker Payment
        public decimal TotalWorkerPayment { get; set; }

        // Expenses
        public decimal TotalExpenses { get; set; }
        public List<CategoryExpenseSummary> ExpenseByCategory { get; set; } = new();

        // Maintenance
        public decimal TotalMaintenance { get; set; }

        // Thread
        public decimal ThreadUsedThisPeriod { get; set; }

        // Machines
        public int ActiveMachinesCount { get; set; }
        public int TotalMachinesCount { get; set; }

        // Work Orders
        public List<WorkOrderPendingSummary> PendingWorkOrders { get; set; } = new();

        // Profit
        public decimal TotalPartyRevenue { get; set; }
        public decimal EstimatedProfit { get; set; }
    }

    public class WorkerProductionSummary
    {
        public string? WorkerName { get; set; }
        public int Pieces { get; set; }
        public decimal Earning { get; set; }
    }

    public class CategoryExpenseSummary
    {
        public string? CategoryName { get; set; }
        public decimal Amount { get; set; }
    }

    public class WorkOrderPendingSummary
    {
        public string? PartyName { get; set; }
        public string? GarmentName { get; set; }
        public int TotalPieces { get; set; }
        public int CompletedPieces { get; set; }
        public int PendingPieces { get; set; }
    }
}