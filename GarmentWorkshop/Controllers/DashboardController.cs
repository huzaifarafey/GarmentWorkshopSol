using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarmentWorkshop.EF;
using GarmentWorkshop.Models;
using GarmentWorkshop.ViewModels;

namespace GarmentWorkshop.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Dashboard?period=today|week|15days|month|custom&from=&to=
        public async Task<IActionResult> Index(string period = "today", DateTime? from = null, DateTime? to = null)
        {
            var (fromDate, toDate, label) = ResolvePeriod(period, from, to);

            var vm = await BuildDashboard(fromDate, toDate, label);

            ViewBag.SelectedPeriod = period;
            return View(vm);
        }

        private (DateTime, DateTime, string) ResolvePeriod(string period, DateTime? from, DateTime? to)
        {
            var today = DateTime.Today;

            return period switch
            {
                "today" => (today, today, "Today"),
                "week" => (today.AddDays(-6), today, "Last 7 Days"),
                "15days" => (today.AddDays(-14), today, "Last 15 Days"),
                "month" => (today.AddDays(-29), today, "Last 30 Days"),
                "custom" when from.HasValue && to.HasValue => (from.Value.Date, to.Value.Date, $"{from.Value:dd-MMM-yyyy} to {to.Value:dd-MMM-yyyy}"),
                _ => (today, today, "Today")
            };
        }

        private async Task<DashboardViewModel> BuildDashboard(DateTime fromDate, DateTime toDate, string label)
        {
            var vm = new DashboardViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                PeriodLabel = label
            };

            // ---------- Production + Worker Earnings ----------
            var productions = await _context.Productions
                .Include(p => p.Worker)
                .Include(p => p.WorkOrder).ThenInclude(wo => wo.Garment)
                .Where(p => p.Date >= fromDate && p.Date <= toDate)
                .ToListAsync();

            vm.TotalPiecesProduced = productions.Sum(p => p.PiecesProduced);

            var workerGroups = productions.GroupBy(p => p.Worker.Name);
            foreach (var group in workerGroups)
            {
                decimal earning = 0;
                int pieces = 0;

                foreach (var p in group)
                {
                    var rate = await GetApplicableRate(p.WorkerId, p.WorkOrder.GarmentId, p.Date);
                    earning += rate * p.PiecesProduced;
                    pieces += p.PiecesProduced;
                }

                vm.WorkerWiseProduction.Add(new WorkerProductionSummary
                {
                    WorkerName = group.Key,
                    Pieces = pieces,
                    Earning = earning
                });
            }

            vm.TotalWorkerPayment = vm.WorkerWiseProduction.Sum(w => w.Earning);

            // ---------- Expenses ----------
            var expenses = await _context.Expenses
                .Include(e => e.ExpenseCategory)
                .Where(e => e.Date >= fromDate && e.Date <= toDate)
                .ToListAsync();

            vm.TotalExpenses = expenses.Sum(e => e.Amount);

            vm.ExpenseByCategory = expenses
                .GroupBy(e => e.ExpenseCategory.Name)
                .Select(g => new CategoryExpenseSummary { CategoryName = g.Key, Amount = g.Sum(e => e.Amount) })
                .ToList();

            // ---------- Maintenance ----------
            vm.TotalMaintenance = await _context.MachineMaintenances
                .Where(mm => mm.Date >= fromDate && mm.Date <= toDate)
                .SumAsync(mm => (decimal?)mm.Amount) ?? 0;

            // ---------- Thread Consumption ----------
            vm.ThreadUsedThisPeriod = await _context.ThreadTransactions
                .Where(tt => tt.Date >= fromDate && tt.Date <= toDate && tt.Type == ThreadTransactionType.Consumption)
                .SumAsync(tt => (decimal?)tt.Quantity) ?? 0;

            // ---------- Machines ----------
            vm.TotalMachinesCount = await _context.Machines.CountAsync();
            vm.ActiveMachinesCount = await _context.Machines.CountAsync(m => m.Status == MachineStatus.Active);

            // ---------- Work Orders: Pending ----------
            var openOrders = await _context.WorkOrders
                .Include(wo => wo.Party)
                .Include(wo => wo.Garment)
                .Where(wo => wo.Status == WorkOrderStatus.Open)
                .ToListAsync();

            foreach (var wo in openOrders)
            {
                var completed = await _context.Productions
                    .Where(p => p.WorkOrderId == wo.Id)
                    .SumAsync(p => (int?)p.PiecesProduced) ?? 0;

                vm.PendingWorkOrders.Add(new WorkOrderPendingSummary
                {
                    PartyName = wo.Party.Name,
                    GarmentName = wo.Garment.Name,
                    TotalPieces = wo.TotalPieces,
                    CompletedPieces = completed,
                    PendingPieces = wo.TotalPieces - completed
                });
            }

            // ---------- Profit (based on production in this period) ----------
            decimal partyRevenue = 0;
            foreach (var p in productions)
            {
                var workOrder = await _context.WorkOrders.FindAsync(p.WorkOrderId);
                partyRevenue += workOrder.PartyRatePerPiece * p.PiecesProduced;
            }

            vm.TotalPartyRevenue = partyRevenue;
            vm.EstimatedProfit = partyRevenue - vm.TotalWorkerPayment - vm.TotalExpenses - vm.TotalMaintenance;

            return vm;
        }

        private async Task<decimal> GetApplicableRate(int workerId, int garmentId, DateTime date)
        {
            var rate = await _context.WorkerRates
                .Where(wr => wr.WorkerId == workerId
                          && wr.GarmentId == garmentId
                          && wr.EffectiveFrom <= date)
                .OrderByDescending(wr => wr.EffectiveFrom)
                .FirstOrDefaultAsync();

            return rate?.RatePerPiece ?? 0;
        }
    }
}