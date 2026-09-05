using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarmentWorkshop.EF;
using GarmentWorkshop.Models;

namespace GarmentWorkshop.Controllers;

public class ProductionController : Controller
{
    private readonly AppDbContext _context;

    public ProductionController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Production
    public async Task<IActionResult> Index()
    {
        var productions = await _context.Productions
            .Include(p => p.Worker)
            .Include(p => p.WorkOrder).ThenInclude(wo => wo.Garment)
            .Include(p => p.WorkOrder).ThenInclude(wo => wo.Party)
            .OrderByDescending(p => p.Date)
            .ToListAsync();

        // Calculate earning for each row (for display only, not stored)
        var earningsMap = new Dictionary<int, decimal>();
        foreach (var p in productions)
        {
            var rate = await GetApplicableRate(p.WorkerId, p.WorkOrder.GarmentId, p.Date);
            earningsMap[p.Id] = rate * p.PiecesProduced;
        }
        ViewBag.Earnings = earningsMap;

        return View(productions);
    }

    // GET: /Production/Create
    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View(new Production { Date = DateTime.Today });
    }

    // POST: /Production/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Production production)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(production.WorkerId, production.WorkOrderId);
            return View(production);
        }

        // Check a rate actually exists for this worker+garment+date before saving,
        // otherwise earnings can't ever be calculated for this record.
        var workOrder = await _context.WorkOrders.FindAsync(production.WorkOrderId);
        var rate = await GetApplicableRate(production.WorkerId, workOrder.GarmentId, production.Date);

        if (rate == 0)
        {
            ModelState.AddModelError("", "No worker rate found for this worker + garment as of this date. Please add a Worker Rate first.");
            await PopulateDropdowns(production.WorkerId, production.WorkOrderId);
            return View(production);
        }

        _context.Productions.Add(production);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Production/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var production = await _context.Productions.FindAsync(id);

        if (production == null)
            return NotFound();

        await PopulateDropdowns(production.WorkerId, production.WorkOrderId);
        return View(production);
    }

    // POST: /Production/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Production production)
    {
        if (id != production.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(production.WorkerId, production.WorkOrderId);
            return View(production);
        }

        _context.Entry(production).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Productions.AnyAsync(p => p.Id == id))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /Production/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var production = await _context.Productions
            .Include(p => p.Worker)
            .Include(p => p.WorkOrder).ThenInclude(wo => wo.Garment)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (production == null)
            return NotFound();

        return View(production);
    }

    // POST: /Production/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var production = await _context.Productions.FindAsync(id);

        if (production != null)
        {
            _context.Productions.Remove(production);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // Core business logic: find the applicable rate for a worker+garment as of a date.
    // "Latest rate where EffectiveFrom <= given date" — historical rates stay correct
    // even after a newer rate is added.
    private async Task<decimal> GetApplicableRate(int workerId, int garmentId, DateTime date)
    {
        var rate = await _context.WorkerRates
            .Where(wr => wr.WorkerId == workerId
                      && wr.GarmentId == garmentId
                      && wr.EffectiveFrom <= date)
            .OrderByDescending(wr => wr.EffectiveFrom)
            .FirstOrDefaultAsync();

        return rate?.RatePerPiece ?? 0; // Return 0 if no applicable rate found
    }

    // Helper: loads Worker/WorkOrder dropdown lists into ViewBag
    private async Task PopulateDropdowns(int? selectedWorkerId = null, int? selectedWorkOrderId = null)
    {
        ViewBag.WorkerId = new SelectList(
            await _context.Workers.Where(w => w.Status == WorkerStatus.Active).ToListAsync(),
            "Id", "Name", selectedWorkerId);

        var workOrders = await _context.WorkOrders
            .Include(wo => wo.Party)
            .Include(wo => wo.Garment)
            .Where(wo => wo.Status == WorkOrderStatus.Open)
            .ToListAsync();

        // Show "Party - Garment" as label so it's identifiable in dropdown
        ViewBag.WorkOrderId = new SelectList(
            workOrders.Select(wo => new { wo.Id, Label = $"{wo.Party.Name} - {wo.Garment.Name}" }),
            "Id", "Label", selectedWorkOrderId);
    }
}