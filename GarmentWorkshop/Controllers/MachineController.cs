using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarmentWorkshop.EF;
using GarmentWorkshop.Models;

namespace GarmentWorkshop.Controllers
{
    public class MachineController : Controller
    {
        private readonly AppDbContext _context;

        public MachineController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Machine
        public async Task<IActionResult> Index()
        {
            var machines = await _context.Machines.ToListAsync();
            return View(machines);
        }

        // GET: /Machine/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Machine/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Machine machine)
        {
            if (!ModelState.IsValid)
                return View(machine);

            _context.Machines.Add(machine);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Machine/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var machine = await _context.Machines.FindAsync(id);

            if (machine == null)
                return NotFound();

            return View(machine);
        }

        // POST: /Machine/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Machine machine)
        {
            if (id != machine.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(machine);

            _context.Entry(machine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Machines.AnyAsync(m => m.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Machine/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var machine = await _context.Machines.FirstOrDefaultAsync(m => m.Id == id);

            if (machine == null)
                return NotFound();

            return View(machine);
        }

        // POST: /Machine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var machine = await _context.Machines.FindAsync(id);

            if (machine != null)
            {
                _context.Machines.Remove(machine);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}