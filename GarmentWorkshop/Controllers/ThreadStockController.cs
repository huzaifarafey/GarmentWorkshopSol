using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarmentWorkshop.EF;
using GarmentWorkshop.Models;

namespace GarmentWorkshop.Controllers
{
    public class ThreadStockController : Controller
    {
        private readonly AppDbContext _context;

        public ThreadStockController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /ThreadStock
        public async Task<IActionResult> Index()
        {
            var stocks = await _context.ThreadStocks.ToListAsync();
            return View(stocks);
        }

        // GET: /ThreadStock/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /ThreadStock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThreadStock stock)
        {
            if (!ModelState.IsValid)
                return View(stock);

            stock.CurrentQuantity = 0; // starts at zero, builds up via transactions
            _context.ThreadStocks.Add(stock);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /ThreadStock/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var stock = await _context.ThreadStocks.FirstOrDefaultAsync(s => s.Id == id);

            if (stock == null)
                return NotFound();

            return View(stock);
        }

        // POST: /ThreadStock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stock = await _context.ThreadStocks.FindAsync(id);

            if (stock != null)
            {
                _context.ThreadStocks.Remove(stock);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}