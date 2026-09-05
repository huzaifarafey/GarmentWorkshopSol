using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarmentWorkshop.EF;
using GarmentWorkshop.Models;

namespace GarmentWorkshop.Controllers
{
    public class WorkerController : Controller
    {
        private readonly AppDbContext _context;

        public WorkerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Worker
        public async Task<IActionResult> Index()
        {
            var workers = await _context.Workers.ToListAsync();
            return View(workers);
        }

        // GET: /Worker/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Worker/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Worker worker)
        {
            if (!ModelState.IsValid)
                return View(worker);

            _context.Workers.Add(worker);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Worker/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var worker = await _context.Workers.FindAsync(id);

            if (worker == null)
                return NotFound();

            return View(worker);
        }

        // POST: /Worker/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Worker worker)
        {
            if (id != worker.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(worker);

            _context.Entry(worker).State = EntityState.Modified; 

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Workers.AnyAsync(w => w.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Worker/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var worker = await _context.Workers.FirstOrDefaultAsync(w => w.Id == id);

            if (worker == null)
                return NotFound();

            return View(worker);
        }

        // POST: /Worker/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var worker = await _context.Workers.FindAsync(id);

            if (worker != null)
            {
                _context.Workers.Remove(worker);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}