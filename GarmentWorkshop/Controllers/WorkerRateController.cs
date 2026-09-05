using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarmentWorkshop.EF;
using GarmentWorkshop.Models;

namespace GarmentWorkshop.Controllers;

public class WorkerRateController : Controller
{
    private readonly AppDbContext _context;

    public WorkerRateController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /WorkerRate
    public async Task<IActionResult> Index()
    {
        var rates = await _context.WorkerRates
            .Include(wr => wr.Worker)
            .Include(wr => wr.Garment)
            .OrderByDescending(wr => wr.EffectiveFrom)
            .ToListAsync();

        return View(rates);
    }

    // GET: /WorkerRate/Create
    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns(); // Load dropdowns for Worker and Garment
        return View();
    }

    // POST: /WorkerRate/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkerRate workerRate)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(workerRate.WorkerId, workerRate.GarmentId);
            return View(workerRate);
        }

        _context.WorkerRates.Add(workerRate);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /WorkerRate/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var workerRate = await _context.WorkerRates.FindAsync(id);

        if (workerRate == null)
            return NotFound();

        await PopulateDropdowns(workerRate.WorkerId, workerRate.GarmentId);
        return View(workerRate);
    }

    // POST: /WorkerRate/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WorkerRate workerRate)
    {
        if (id != workerRate.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(workerRate.WorkerId, workerRate.GarmentId);
            return View(workerRate);
        }

        _context.Entry(workerRate).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.WorkerRates.AnyAsync(wr => wr.Id == id))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /WorkerRate/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var workerRate = await _context.WorkerRates
            .Include(wr => wr.Worker)
            .Include(wr => wr.Garment)
            .FirstOrDefaultAsync(wr => wr.Id == id);

        if (workerRate == null)
            return NotFound();

        return View(workerRate);
    }

    // POST: /WorkerRate/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var workerRate = await _context.WorkerRates.FindAsync(id);

        if (workerRate != null)
        {
            _context.WorkerRates.Remove(workerRate);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // Helper: loads Worker/Garment dropdown lists into ViewBag
    private async Task PopulateDropdowns(int? selectedWorkerId = null, int? selectedGarmentId = null)
    {
        ViewBag.WorkerId = new SelectList(
            await _context.Workers.Where(w => w.Status == WorkerStatus.Active).ToListAsync(),
            "Id", "Name", selectedWorkerId);

        ViewBag.GarmentId = new SelectList(
            await _context.Garments.Where(g => g.Status == GarmentStatus.Active).ToListAsync(),
            "Id", "Name", selectedGarmentId);
    }
}