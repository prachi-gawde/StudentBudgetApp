using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentBudgetApp.Data;
using StudentBudgetApp.Models;

namespace StudentBudgetApp.Controllers
{
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // READ: List all entries & Calculate totals
        public async Task<IActionResult> Index()
        {
            int viewCount = HttpContext.Session.GetInt32("BudgetDashboardViews") ?? 0;
            HttpContext.Session.SetInt32("BudgetDashboardViews", viewCount + 1);
            ViewBag.ViewCount = viewCount + 1;

            var entries = await _context.BudgetEntries
                .OrderByDescending(e => e.Date)
                .ThenByDescending(e => e.Id)
                .ToListAsync();

            decimal totalIncome = entries.Where(e => e.Type == "Income").Sum(e => e.Amount);
            decimal totalExpense = entries.Where(e => e.Type == "Expense").Sum(e => e.Amount);
            decimal netBalance = totalIncome - totalExpense;

            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpense = totalExpense;
            ViewBag.NetBalance = netBalance;

            // Compute expense distribution by category for Chart.js
            var expenseByCategory = entries
                .Where(e => e.Type == "Expense")
                .GroupBy(e => string.IsNullOrWhiteSpace(e.Category) ? "Other" : e.Category.Trim())
                .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) })
                .OrderByDescending(g => g.Total)
                .ToList();

            ViewBag.ExpenseCategories = expenseByCategory.Select(x => x.Category).ToList();
            ViewBag.ExpenseAmounts = expenseByCategory.Select(x => x.Total).ToList();

            return View(entries);
        }

        // CREATE: Get Form
        public IActionResult Create()
        {
            return View();
        }

        // CREATE: Post Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BudgetEntry entry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entry);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(entry);
        }

        // UPDATE: Get Form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.BudgetEntries.FindAsync(id);
            if (entry == null) return NotFound();

            return View(entry);
        }

        // UPDATE: Post Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BudgetEntry entry)
        {
            if (id != entry.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entry);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Transaction updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.BudgetEntries.Any(e => e.Id == entry.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(entry);
        }

        // DELETE: Get Confirmation Page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var entry = await _context.BudgetEntries.FirstOrDefaultAsync(m => m.Id == id);
            if (entry == null) return NotFound();

            return View(entry);
        }

        // DELETE: Confirmed Action
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entry = await _context.BudgetEntries.FindAsync(id);
            if (entry != null)
            {
                _context.BudgetEntries.Remove(entry);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Transaction deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}