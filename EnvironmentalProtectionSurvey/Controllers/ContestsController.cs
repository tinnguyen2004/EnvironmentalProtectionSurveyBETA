using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentalProtectionSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EnvironmentalProtectionSurvey.Controllers
{
    public class ContestsController : Controller
    {
        private readonly Project2Context _context;

        public ContestsController(Project2Context context)
        {
            _context = context;
        }

        // GET: Contests
        public async Task<IActionResult> Index()
        {
            return _context.Contests != null ?
                        View(await _context.Contests.ToListAsync()) :
                        Problem("Entity set 'Project2Context.Contests'  is null.");
        }

        // GET: Contests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Contests == null)
            {
                return NotFound();
            }

            var contest = await _context.Contests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            return View(contest);
        }

        // GET: Contests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,StartTime,EndTime")] Contest contest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contest);
                await _context.SaveChangesAsync();
                // Display success alert using SweetAlert2
                return Json(new { success = true });
            }
            return View(contest);
        }

        // GET: Contests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Contests == null)
            {
                return NotFound();
            }

            var contest = await _context.Contests.FindAsync(id);
            if (contest == null)
            {
                return NotFound();
            }
            return View(contest);
        }

        // POST: Contests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,StartTime,EndTime")] Contest contest)
        {
            if (id != contest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContestExists(contest.Id))
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
            return View(contest);
        }

        public IActionResult Participated()
        {
            ViewBag.participated = "You have participated in this survey";
            return View();
        }

        // GET: Contests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contests == null)
            {
                return NotFound();
            }

            var contest = await _context.Contests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contest == null)
            {
                return NotFound();
            }

            return View(contest);
        }

        // POST: Contests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Contests == null)
            {
                return Problem("Entity set 'SurveyProjectContext.Contests'  is null.");
            }
            var contest = await _context.Contests.FindAsync(id);
            if (contest != null)
            {
                _context.Contests.Remove(contest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Contest/TakeContest/1
        //public IActionResult TakeContest(int id)
        //{
        //    var contest = _context.Contests
        //        .Include(c => c.QuestionContests)
        //        .FirstOrDefault(c => c.Id == id);

        //    if (contest == null)
        //    {
        //        return NotFound();
        //    }

        //    // Check if contest is closed
        //    if (contest.EndTime < DateTime.Now)
        //    {
        //        return View("Closed", contest);
        //    }

        //    return View(contest);
        //}

        public IActionResult TakeContest(int id)
        {
            var FilledContest = _context.Winners.FirstOrDefault(f => f.ContestId == id);
            if (FilledContest == null)
            {
                var contest = _context.Contests
                .Include(c => c.QuestionContests)
                .FirstOrDefault(c => c.Id == id);
                if (contest == null)
                {
                    // Handle the case where the survey is not found
                    return NotFound();
                }
                // Check if contest is closed
                else if (contest.EndTime < DateTime.Now)
                {
                    return View("Closed", contest);
                }
                return View(contest);
            }
            else
            {
                return RedirectToAction(nameof(Participated));
            }
        }

        // POST: /Contest/TakeContest/1
        [HttpPost]
        public IActionResult TakeContest(int id, Dictionary<int, string[]> selectedOptions)
        {
            var username = HttpContext.Session.GetString("username");
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            var contest = _context.Contests
                .Include(c => c.QuestionContests)
                .FirstOrDefault(c => c.Id == id);

            if (contest == null)
            {
                return NotFound();
            }

            // Check if contest is closed
            if (contest.EndTime < DateTime.Now)
            {
                return View("Closed", contest);
            }

            var results = new List<ResultViewModel>(); // Create a list to store results

            foreach (var question in contest.QuestionContests)
            {
                string correctAnswer = question.CorrectAnswer; // assuming CorrectAnswer is a string
                string[] selectedOptionsForQuestion;

                // Kiểm tra xem người dùng đã chọn câu trả lời cho câu hỏi này hay không
                if (selectedOptions.TryGetValue(question.Id, out selectedOptionsForQuestion))
                {
                    // So sánh câu trả lời đã chọn với câu trả lời đúng của câu hỏi
                    bool isCorrect = selectedOptionsForQuestion != null && selectedOptionsForQuestion.Contains(correctAnswer);

                    // Add result to the list
                    results.Add(new ResultViewModel
                    {
                        QuestionText = question.QuestionText,
                        UserAnswer = selectedOptionsForQuestion != null ? string.Join(", ", selectedOptionsForQuestion) : "No answer",
                        IsCorrect = isCorrect
                    });
                }
            }
            // Nếu không có câu trả lời nào được chọn hoặc không có câu trả lời nào đúng
            //return View(contest);
            // Pass the results to the Result view
            foreach (var item in selectedOptions)
            {
                var filledcontest = new Winner
                {
                    ContestId = contest.Id,
                    UserId = user.Id
                };
                _context.Winners.Add(filledcontest);
                _context.SaveChanges();
            }
            return View("Result", results);
        }

        private bool ContestExists(int id)
        {
            return (_context.Contests?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
