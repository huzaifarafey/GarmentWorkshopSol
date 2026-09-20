using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarmentWorkshop.EF;
using GarmentWorkshop.Models;
using GarmentWorkshop.ViewModels;

namespace GarmentWorkshop.Controllers
{
    public class MasterDataController : Controller
    {
        private readonly AppDbContext _context;

        public MasterDataController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new MasterDataViewModel
            {
                Workers = await _context.Workers.ToListAsync(),
                Parties = await _context.Parties.ToListAsync(),
                Machines = await _context.Machines.ToListAsync()
            };

            ViewBag.ActiveTab = TempData["ActiveTab"] as string ?? "worker";
            ViewBag.Message = TempData["Message"] as string;
            ViewBag.MessageType = TempData["MessageType"] as string ?? "info";

            return View(vm);
        }

        // ---------- Worker ----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWorker(Worker worker)
        {
            _context.Workers.Add(worker);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Worker \"{worker.Name}\" added.";
            TempData["MessageType"] = "success";
            TempData["ActiveTab"] = "worker";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWorker(Worker worker)
        {
            _context.Entry(worker).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Worker \"{worker.Name}\" updated.";
            TempData["MessageType"] = "success";
            TempData["ActiveTab"] = "worker";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorker(int id)
        {
            var worker = await _context.Workers.FindAsync(id);
            if (worker != null)
            {
                try
                {
                    _context.Workers.Remove(worker);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"Worker \"{worker.Name}\" deleted.";
                    TempData["MessageType"] = "success";
                }
                catch (DbUpdateException)
                {
                    TempData["Message"] = $"Cannot delete \"{worker.Name}\" — they have production or rate records. Set Status to Inactive instead.";
                    TempData["MessageType"] = "danger";
                }
            }
            TempData["ActiveTab"] = "worker";
            return RedirectToAction(nameof(Index));
        }

        // ---------- Party ----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddParty(Party party)
        {
            _context.Parties.Add(party);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Party \"{party.Name}\" added.";
            TempData["MessageType"] = "success";
            TempData["ActiveTab"] = "party";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditParty(Party party)
        {
            _context.Entry(party).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Party \"{party.Name}\" updated.";
            TempData["MessageType"] = "success";
            TempData["ActiveTab"] = "party";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteParty(int id)
        {
            var party = await _context.Parties.FindAsync(id);
            if (party != null)
            {
                try
                {
                    _context.Parties.Remove(party);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"Party \"{party.Name}\" deleted.";
                    TempData["MessageType"] = "success";
                }
                catch (DbUpdateException)
                {
                    TempData["Message"] = $"Cannot delete \"{party.Name}\" — they have work orders. Set Status to Inactive instead.";
                    TempData["MessageType"] = "danger";
                }
            }
            TempData["ActiveTab"] = "party";
            return RedirectToAction(nameof(Index));
        }

        // ---------- Machine ----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMachine(Machine machine)
        {
            _context.Machines.Add(machine);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Machine \"{machine.Name}\" added.";
            TempData["MessageType"] = "success";
            TempData["ActiveTab"] = "machine";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMachine(Machine machine)
        {
            _context.Entry(machine).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            TempData["Message"] = $"Machine \"{machine.Name}\" updated.";
            TempData["MessageType"] = "success";
            TempData["ActiveTab"] = "machine";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMachine(int id)
        {
            var machine = await _context.Machines.FindAsync(id);
            if (machine != null)
            {
                try
                {
                    _context.Machines.Remove(machine);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"Machine \"{machine.Name}\" deleted.";
                    TempData["MessageType"] = "success";
                }
                catch (DbUpdateException)
                {
                    TempData["Message"] = $"Cannot delete \"{machine.Name}\" — it has maintenance records. Set Status to Inactive instead.";
                    TempData["MessageType"] = "danger";
                }
            }
            TempData["ActiveTab"] = "machine";
            return RedirectToAction(nameof(Index));
        }
    }
}