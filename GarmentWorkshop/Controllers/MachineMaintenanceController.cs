using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarmentWorkshop.EF;
using GarmentWorkshop.Models;

namespace GarmentWorkshop.Controllers
{
    public class MachineMaintenanceController : Controller
    {
        private readonly AppDbContext _context;

        public MachineMaintenanceController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /MachineMaintenance
        public async Task<IActionResult> Index()
        {
            var records = await _context.MachineMaintenances
                .Include(mm => mm.Machine)
                .OrderByDescending(mm => mm.Date)
                .ToListAsync();

            return View(records);
        }

        // GET: /MachineMaintenance/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdown();
            return View(new MachineMaintenance { Date = DateTime.Today });
        }

        // POST: /MachineMaintenance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MachineMaintenance maintenance)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdown(maintenance.MachineId);
                return View(maintenance);
            }

            _context.MachineMaintenances.Add(maintenance);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /MachineMaintenance/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var maintenance = await _context.MachineMaintenances.FindAsync(id);

            if (maintenance == null)
                return NotFound();

            await PopulateDropdown(maintenance.MachineId);
            return View(maintenance);
        }

        // POST: /MachineMaintenance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MachineMaintenance maintenance)
        {
            if (id != maintenance.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateDropdown(maintenance.MachineId);
                return View(maintenance);
            }

            _context.Entry(maintenance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.MachineMaintenances.AnyAsync(mm => mm.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /MachineMaintenance/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var maintenance = await _context.MachineMaintenances
                .Include(mm => mm.Machine)
                .FirstOrDefaultAsync(mm => mm.Id == id);

            if (maintenance == null)
                return NotFound();

            return View(maintenance);
        }

        // POST: /MachineMaintenance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maintenance = await _context.MachineMaintenances.FindAsync(id);

            if (maintenance != null)
            {
                _context.MachineMaintenances.Remove(maintenance);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper: loads Machine dropdown into ViewBag
        private async Task PopulateDropdown(int? selectedMachineId = null)
        {
            ViewBag.MachineId = new SelectList(
                await _context.Machines.Where(m => m.Status == MachineStatus.Active).ToListAsync(),
                "Id", "Name", selectedMachineId);
        }
    }
}