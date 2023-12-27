using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnvironmentalProtectionSurvey.Models;

namespace EnvironmentalProtectionSurvey.Controllers
{
    public class OptionsController : Controller
    {
        private readonly Project2Context _context;

        public OptionsController(Project2Context context)
        {
            _context = context;
        }

        // GET: Options
        public async Task<IActionResult> Index()
        {
            var surveyProjectContext = _context.Options.Include(o => o.Question);
            return View(await surveyProjectContext.ToListAsync());
        }

        // GET: Options/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Options == null)
            {
                return NotFound();
            }

            var option = await _context.Options
                .Include(o => o.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (option == null)
            {
                return NotFound();
            }

            return View(option);
        }

        // GET: Options/Create
        public IActionResult Create()
        {
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Title");
            return View();
        }

        // POST: Options/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,QuestionId")] Option option)
        {
            if (ModelState.IsValid)
            {
                _context.Add(option);
                await _context.SaveChangesAsync();
                // Display success alert using SweetAlert2
                return Json(new { success = true });
            }
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Title", option.QuestionId);
            return View(option);
        }

        // GET: Options/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Options == null)
            {
                return NotFound();
            }

            var option = await _context.Options.FindAsync(id);
            if (option == null)
            {
                return NotFound();
            }
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Title", option.QuestionId);
            return View(option);
        }

        // POST: Options/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,QuestionId")] Option option)
        {
            if (id != option.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(option);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OptionExists(option.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Title", option.QuestionId);
            return View(option);
        }

        // GET: Options/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Options == null)
            {
                return NotFound();
            }

            var option = await _context.Options
                .Include(o => o.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (option == null)
            {
                return NotFound();
            }

            return View(option);
        }

        // POST: Options/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Options == null)
            {
                return Problem("Entity set 'SurveyProjectContext.Options'  is null.");
            }
            var option = await _context.Options.FindAsync(id);
            if (option != null)
            {
                _context.Options.Remove(option);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OptionExists(int id)
        {
          return (_context.Options?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
