using GarmentWorkshop.EF;
using GarmentWorkshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarmentWorkshop.Controllers;

public class PartyController : Controller
{
    private readonly AppDbContext _context;

    public PartyController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Party
    public async Task<IActionResult> Index()
    {
        var parties = await _context.Parties.ToListAsync();
        return View(parties);
    }

    // GET: /Party/Create
    public async Task<IActionResult> Create()
    {
        return View();
    }
    // POST: /Party/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Party party)
    {
        if (!ModelState.IsValid)
        {
            return View(party);
        }

        _context.Parties.Add(party);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    // GET: /Party/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var party = await _context.Parties.FindAsync(id);

        if (party == null)
        {
            return NotFound();
        }
        return View(party);
    }

    // POST: /Party/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Party party)
    {
        if (id != party.Id)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return View(party);
        }

        _context.Entry(party).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Parties.AnyAsync(e => e.Id == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return RedirectToAction("Index");
    }
    // GET: /Party/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var party = await _context.Parties.FirstOrDefaultAsync(p => p.Id == id);
        if (party == null)
        {
            return NotFound();
        }
        return View(party);
    }

    // POST: /Party/Delete/5
    [HttpPost, ActionName("Delete")] // ActionName attribute allows the method to be called when a POST request is made to the /Party/Delete/5 URL.
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var party = await _context.Parties.FindAsync(id);

        if (party != null)
        {
            _context.Parties.Remove(party);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

}
