using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarmentWorkshop.EF;
using GarmentWorkshop.Models;
using GarmentWorkshop.ViewModels;

namespace GarmentWorkshop.Controllers
{
    public class DailyEntryController : Controller
    {
        private readonly AppDbContext _context;

        public DailyEntryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /DailyEntry
        public async Task<IActionResult> Index()
        {
            var vm = new DailyEntryViewModel();
            await PopulateDropdowns(vm);
            await PopulateTodayEntries(vm);

            ViewBag.ActiveTab = TempData["ActiveTab"] as string ?? "production";
            ViewBag.Message = TempData["Message"] as string;

            return View(vm);
        }

        // POST: /DailyEntry/AddProduction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduction(Production productionEntry)
        {
            var workOrder = await _context.WorkOrders.FindAsync(productionEntry.WorkOrderId);
            var rate = await GetApplicableRate(productionEntry.WorkerId, workOrder.GarmentId, productionEntry.Date);

            if (rate == 0)
            {
                TempData["Message"] = "Error: No worker rate found for this worker + garment as of this date.";
                TempData["ActiveTab"] = "production";
                return RedirectToAction(nameof(Index));
            }

            _context.Productions.Add(productionEntry);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Production saved — {productionEntry.PiecesProduced} pieces, ₹{rate * productionEntry.PiecesProduced} earned.";
            TempData["ActiveTab"] = "production";
            return RedirectToAction(nameof(Index));
        }

        // POST: /DailyEntry/AddExpense
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExpense(Expense expenseEntry)
        {
            _context.Expenses.Add(expenseEntry);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Expense of ₹{expenseEntry.Amount} saved.";
            TempData["ActiveTab"] = "expense";
            return RedirectToAction(nameof(Index));
        }

        // POST: /DailyEntry/AddMaintenance
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMaintenance(MachineMaintenance maintenanceEntry)
        {
            _context.MachineMaintenances.Add(maintenanceEntry);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Maintenance of ₹{maintenanceEntry.Amount} saved.";
            TempData["ActiveTab"] = "maintenance";
            return RedirectToAction(nameof(Index));
        }

        // POST: /DailyEntry/AddThread
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddThread(ThreadTransaction threadEntry)
        {
            var stock = await _context.ThreadStocks.FindAsync(threadEntry.ThreadStockId);

            if (threadEntry.Type == ThreadTransactionType.Purchase)
                stock.CurrentQuantity += threadEntry.Quantity;
            else
                stock.CurrentQuantity -= threadEntry.Quantity;

            _context.ThreadTransactions.Add(threadEntry);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"Thread {threadEntry.Type} of {threadEntry.Quantity} recorded. Current stock: {stock.CurrentQuantity}.";
            TempData["ActiveTab"] = "thread";
            return RedirectToAction(nameof(Index));
        }

        // Same rate-lookup logic as ProductionController
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

        private async Task PopulateDropdowns(DailyEntryViewModel vm)
        {
            vm.Workers = new SelectList(
                await _context.Workers.Where(w => w.Status == WorkerStatus.Active).ToListAsync(),
                "Id", "Name");

            var workOrders = await _context.WorkOrders
                .Include(wo => wo.Party)
                .Include(wo => wo.Garment)
                .Where(wo => wo.Status == WorkOrderStatus.Open)
                .ToListAsync();

            vm.WorkOrders = new SelectList(
                workOrders.Select(wo => new { wo.Id, Label = $"{wo.Party.Name} - {wo.Garment.Name}" }),
                "Id", "Label");

            vm.ExpenseCategories = new SelectList(
                await _context.ExpenseCategories.ToListAsync(),
                "Id", "Name");

            vm.Machines = new SelectList(
                await _context.Machines.Where(m => m.Status == MachineStatus.Active).ToListAsync(),
                "Id", "Name");

            vm.ThreadStocks = new SelectList(
                await _context.ThreadStocks.ToListAsync(),
                "Id", "ColorOrType");
        }

        private async Task PopulateTodayEntries(DailyEntryViewModel vm)
        {
            var today = DateTime.Today;
            var entries = new List<RecentEntry>();

            var productions = await _context.Productions
                .Include(p => p.Worker)
                .Include(p => p.WorkOrder).ThenInclude(wo => wo.Garment)
                .Where(p => p.Date == today)
                .ToListAsync();

            foreach (var p in productions)
            {
                var rate = await GetApplicableRate(p.WorkerId, p.WorkOrder.GarmentId, p.Date);
                entries.Add(new RecentEntry
                {
                    Type = "Production",
                    Description = $"{p.Worker.Name} - {p.WorkOrder.Garment.Name}",
                    Value = $"{p.PiecesProduced} pcs — ₹{rate * p.PiecesProduced}"
                });
            }

            var expenses = await _context.Expenses
                .Include(e => e.ExpenseCategory)
                .Where(e => e.Date == today)
                .ToListAsync();

            entries.AddRange(expenses.Select(e => new RecentEntry
            {
                Type = "Expense",
                Description = e.ExpenseCategory.Name + (string.IsNullOrEmpty(e.Note) ? "" : $" ({e.Note})"),
                Value = $"₹{e.Amount}"
            }));

            var maintenances = await _context.MachineMaintenances
                .Include(mm => mm.Machine)
                .Where(mm => mm.Date == today)
                .ToListAsync();

            entries.AddRange(maintenances.Select(mm => new RecentEntry
            {
                Type = "Maintenance",
                Description = mm.Machine.Name + (string.IsNullOrEmpty(mm.Note) ? "" : $" ({mm.Note})"),
                Value = $"₹{mm.Amount}"
            }));

            var threads = await _context.ThreadTransactions
                .Include(tt => tt.ThreadStock)
                .Where(tt => tt.Date == today)
                .ToListAsync();

            entries.AddRange(threads.Select(tt => new RecentEntry
            {
                Type = "Thread",
                Description = $"{tt.ThreadStock.ColorOrType} - {tt.Type}",
                Value = $"{tt.Quantity}"
            }));

            vm.TodayEntries = entries;
        }
    }
}