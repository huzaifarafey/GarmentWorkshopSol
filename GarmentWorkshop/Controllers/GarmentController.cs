using GarmentWorkshop.EF;
using GarmentWorkshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarmentWorkshop.Controllers;

public class GarmentController : Controller
{
    private readonly AppDbContext _context;
    public GarmentController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Garment
    public async Task<IActionResult> Index()
    {
        var garments = await _context.Garments.ToListAsync();
        return View(garments);
    }

    // GET: /Garment/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Garment/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Garment garment)
    {
        if (garment == null)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return View(garment);
        }
        await _context.Garments.AddAsync(garment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // GET: /Garment/Edit
    public async Task<IActionResult> Edit(int id)
    {
        var garment = await _context.Garments.FindAsync(id);
        if (garment == null)
        {
            return NotFound();
        }
        return View(garment);
    }

    // POST: /Garment/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Garment garment)
    {
        if (id != garment.Id)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return View(garment); 
        }

        _context.Entry(garment).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Garments.AnyAsync(g => g.Id == id))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction("Index");
    }

    // GET: /Garment/Delete/5

    public async Task<IActionResult> Delete(int id)
    {
        var garment = await _context.Garments.FirstOrDefaultAsync(g => g.Id == id);
       
        if (garment == null)
        {
            return BadRequest();
        }

        return View(garment);
    }

    // POST: /Garment/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var garment = await _context.Garments.FindAsync(id);
        if (garment != null)
        {
             _context.Garments.Remove(garment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

}
