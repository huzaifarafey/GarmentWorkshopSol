using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GarmentWorkshop.EF;
using GarmentWorkshop.Models;

namespace GarmentWorkshop.Controllers;

public class WorkOrderController : Controller
{
    private readonly AppDbContext _context;

    public WorkOrderController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /WorkOrder
    public async Task<IActionResult> Index()
    {
        var workOrders = await _context.WorkOrders
            .Include(wo => wo.Party)
            .Include(wo => wo.Garment)
            .OrderByDescending(wo => wo.StartDate)
            .ToListAsync();

        return View(workOrders);
    }

    // GET: /WorkOrder/Create
    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View();
    }

    // POST: /WorkOrder/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkOrder workOrder)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(workOrder.PartyId, workOrder.GarmentId);
            return View(workOrder);
        }

        _context.WorkOrders.Add(workOrder);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /WorkOrder/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var workOrder = await _context.WorkOrders.FindAsync(id);

        if (workOrder == null)
            return NotFound();

        await PopulateDropdowns(workOrder.PartyId, workOrder.GarmentId);
        return View(workOrder);
    }

    // POST: /WorkOrder/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WorkOrder workOrder)
    {
        if (id != workOrder.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(workOrder.PartyId, workOrder.GarmentId);
            return View(workOrder);
        }

        _context.Entry(workOrder).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.WorkOrders.AnyAsync(wo => wo.Id == id))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /WorkOrder/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var workOrder = await _context.WorkOrders
            .Include(wo => wo.Party)
            .Include(wo => wo.Garment)
            .FirstOrDefaultAsync(wo => wo.Id == id);

        if (workOrder == null)
            return NotFound();

        return View(workOrder);
    }

    // POST: /WorkOrder/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var workOrder = await _context.WorkOrders.FindAsync(id);

        if (workOrder != null)
        {
            _context.WorkOrders.Remove(workOrder);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: /WorkOrder/Details/5 — shows pending/completed calculation
    public async Task<IActionResult> Details(int id)
    {
        var workOrder = await _context.WorkOrders
            .Include(wo => wo.Party)
            .Include(wo => wo.Garment)
            .FirstOrDefaultAsync(wo => wo.Id == id);

        if (workOrder == null)
            return NotFound();

        var completedPieces = await _context.Productions
            .Where(p => p.WorkOrderId == id)
            .SumAsync(p => (int?)p.PiecesProduced) ?? 0;

        ViewBag.CompletedPieces = completedPieces;
        ViewBag.PendingPieces = workOrder.TotalPieces - completedPieces;

        return View(workOrder);
    }

    // Helper: loads Party/Garment dropdown lists into ViewBag
    private async Task PopulateDropdowns(int? selectedPartyId = null, int? selectedGarmentId = null)
    {
        ViewBag.PartyId = new SelectList(
            await _context.Parties.Where(p => p.Status == PartyStatus.Active).ToListAsync(),
            "Id", "Name", selectedPartyId);

        ViewBag.GarmentId = new SelectList(
            await _context.Garments.Where(g => g.Status == GarmentStatus.Active).ToListAsync(),
            "Id", "Name", selectedGarmentId);
    }
}