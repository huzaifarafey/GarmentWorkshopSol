using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarmentWorkshop.EF;
using GarmentWorkshop.Models;

namespace GarmentWorkshop.Controllers
{
    public class ExpenseCategoryController : Controller
    {
        private readonly AppDbContext _context;

        public ExpenseCategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /ExpenseCategory
        public async Task<IActionResult> Index()
        {
            var categories = await _context.ExpenseCategories.ToListAsync();
            return View(categories);
        }

        // GET: /ExpenseCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /ExpenseCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseCategory category)
        {
            if (!ModelState.IsValid)
                return View(category);

            _context.ExpenseCategories.Add(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /ExpenseCategory/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.ExpenseCategories.FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: /ExpenseCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.ExpenseCategories.FindAsync(id);

            if (category != null)
            {
                _context.ExpenseCategories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
