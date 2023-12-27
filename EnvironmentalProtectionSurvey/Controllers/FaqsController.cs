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
    public class FaqsController : Controller
    {
        private readonly Project2Context _context;

        public FaqsController(Project2Context context)
        {
            _context = context;
        }

        // GET: Faqs
        public async Task<IActionResult> Index()
        {
              return _context.Faqs != null ? 
                          View(await _context.Faqs.ToListAsync()) :
                          Problem("Entity set 'Project2Context.Faqs'  is null.");
        }

        // GET: Faqs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Faqs == null)
            {
                return NotFound();
            }

            var faq = await _context.Faqs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (faq == null)
            {
                return NotFound();
            }

            return View(faq);
        }

        // GET: Faqs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Faqs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Answer")] Faq faq)
        {
            if (ModelState.IsValid)
            {
                _context.Add(faq);
                await _context.SaveChangesAsync();
                // Display success alert using SweetAlert2
                return Json(new { success = true });
            }
            return View(faq);
        }

        // GET: Faqs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Faqs == null)
            {
                return NotFound();
            }

            var faq = await _context.Faqs.FindAsync(id);
            if (faq == null)
            {
                return NotFound();
            }
            return View(faq);
        }

        // POST: Faqs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Answer")] Faq faq)
        {
            if (id != faq.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faq);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FaqExists(faq.Id))
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
            return View(faq);
        }

        // GET: Faqs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Faqs == null)
            {
                return NotFound();
            }

            var faq = await _context.Faqs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (faq == null)
            {
                return NotFound();
            }

            return View(faq);
        }

        // POST: Faqs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Faqs == null)
            {
                return Problem("Entity set 'SurveyProjectContext.Faqs'  is null.");
            }
            var faq = await _context.Faqs.FindAsync(id);
            if (faq != null)
            {
                _context.Faqs.Remove(faq);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FaqExists(int id)
        {
          return (_context.Faqs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
