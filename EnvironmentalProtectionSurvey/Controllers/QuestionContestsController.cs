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
    public class QuestionContestsController : Controller
    {
        private readonly Project2Context _context;

        public QuestionContestsController(Project2Context context)
        {
            _context = context;
        }

        // GET: QuestionContests
        public async Task<IActionResult> Index()
        {
            var project2Context = _context.QuestionContests.Include(q => q.Contest);
            return View(await project2Context.ToListAsync());
        }

        // GET: QuestionContests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.QuestionContests == null)
            {
                return NotFound();
            }

            var questionContest = await _context.QuestionContests
                .Include(q => q.Contest)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (questionContest == null)
            {
                return NotFound();
            }

            return View(questionContest);
        }

        // GET: QuestionContests/Create
        public IActionResult Create()
        {
            ViewData["ContestId"] = new SelectList(_context.Contests, "Id", "Title");
            return View();
        }

        // POST: QuestionContests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContestId,QuestionText,AnswerOptions,CorrectAnswer")] QuestionContest questionContest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(questionContest);
                await _context.SaveChangesAsync();
                // Display success alert using SweetAlert2
                return Json(new { success = true });
            }
            ViewData["ContestId"] = new SelectList(_context.Contests, "Id", "Title", questionContest.ContestId);
            return View(questionContest);
        }

        // GET: QuestionContests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.QuestionContests == null)
            {
                return NotFound();
            }

            var questionContest = await _context.QuestionContests.FindAsync(id);
            if (questionContest == null)
            {
                return NotFound();
            }
            ViewData["ContestId"] = new SelectList(_context.Contests, "Id", "Title", questionContest.ContestId);
            return View(questionContest);
        }

        // POST: QuestionContests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ContestId,QuestionText,AnswerOptions,CorrectAnswer")] QuestionContest questionContest)
        {
            if (id != questionContest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(questionContest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionContestExists(questionContest.Id))
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
            ViewData["ContestId"] = new SelectList(_context.Contests, "Id", "Title", questionContest.ContestId);
            return View(questionContest);
        }

        // GET: QuestionContests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.QuestionContests == null)
            {
                return NotFound();
            }

            var questionContest = await _context.QuestionContests
                .Include(q => q.Contest)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (questionContest == null)
            {
                return NotFound();
            }

            return View(questionContest);
        }

        // POST: QuestionContests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.QuestionContests == null)
            {
                return Problem("Entity set 'SurveyProjectContext.QuestionContests'  is null.");
            }
            var questionContest = await _context.QuestionContests.FindAsync(id);
            if (questionContest != null)
            {
                _context.QuestionContests.Remove(questionContest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionContestExists(int id)
        {
          return (_context.QuestionContests?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
