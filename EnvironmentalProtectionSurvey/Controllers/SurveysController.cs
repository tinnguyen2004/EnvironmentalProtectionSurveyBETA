using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using Microsoft.Extensions.Options;
using EnvironmentalProtectionSurvey.Models;

namespace EnvironmentalProtectionSurvey.Controllers
{
    public class SurveysController : Controller
    {
        private readonly Project2Context _context;

        public SurveysController(Project2Context context)
        {
            _context = context;
        }

        // GET: Surveys
        public async Task<IActionResult> Index()
        {
            return _context.Surveys != null ?
                        View(await _context.Surveys.ToListAsync()) :
                        Problem("Entity set 'SurveyProjectContext.Surveys'  is null.");
        }

        // GET: Surveys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Surveys == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys
                .FirstOrDefaultAsync(m => m.Id == id);
            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }

        // GET: Surveys/ViewQuestions/5
        public async Task<IActionResult> Question(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questions = await _context.Questions
                .Include(q => q.Options)
                .Where(q => q.SurveyId == id)
                .ToListAsync();

            if (questions == null || !questions.Any())
            {
                return NotFound();
            }

            var options = await _context.Options.ToListAsync();

            // Create and populate the view model
            var viewModel = new QuestionOptionsViewModel(questions, options);

            // Return the view with the view model
            return View(viewModel);
        }

        // GET: Surveys/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Surveys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,UserType,Form,UserPost,CreatedAt,EndAt")] Survey survey)
        {
            if (ModelState.IsValid)
            {
                _context.Add(survey);
                await _context.SaveChangesAsync();
                // Display success alert using SweetAlert2
                return Json(new { success = true });
            }
            return View(survey);
        }

        public async Task<IActionResult> Hide(int? id)
        {
            if (id == null || _context.Surveys == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys
                .FirstOrDefaultAsync(m => m.Id == id);
            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Hide")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Hide(int id)
        {
            if (_context.Surveys == null)
            {
                return Problem("Entity set 'SurveyProjectContext.Surveys'  is null.");
            }
            var survey = await _context.Surveys.FindAsync(id);
            survey.IsVisible = false;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurveyExists(int id)
        {
            return (_context.Surveys?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public async Task<IActionResult> Edit(int id)
        {
            // Check if the survey exists
            var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.Id == id);
            if (survey == null)
            {
                return NotFound();
            }

            // Get the questions for the survey
            var questions = survey.Questions;

            // Display the edit form
            return View(survey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Survey survey, List<Question> questions)
        {
            // Check if the survey exists
            var existingSurvey = await _context.Surveys.FirstOrDefaultAsync(s => s.Id == id);
            if (existingSurvey == null)
            {
                return NotFound();
            }

            // Update the survey details
            existingSurvey.Title = survey.Title;
            existingSurvey.UserType = survey.UserType;
            existingSurvey.UserPost = survey.UserPost;
            existingSurvey.CreatedAt = DateTime.Now;
            existingSurvey.EndAt = survey.EndAt;

            // Update the questions
            foreach (var question in questions)
            {
                var existingQuestion = existingSurvey.Questions.FirstOrDefault(q => q.Id == question.Id);
                if (existingQuestion == null)
                {
                    existingQuestion = new Question();
                    existingSurvey.Questions.Add(existingQuestion);
                }
                existingQuestion.Title = question.Title;
                existingQuestion.CorrectAnswer = question.CorrectAnswer;

                // Update the options
                foreach (var option in question.Options)
                {
                    var existingOption = existingQuestion.Options.FirstOrDefault(o => o.Id == option.Id);
                    if (existingOption == null)
                    {
                        existingOption = new Option();
                        existingQuestion.Options.Add(existingOption);
                    }
                    existingOption.Title = option.Title;
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult TakeSurvey(int id)
        {
            var survey = _context.Surveys.Include(s => s.Questions).ThenInclude(q => q.Options).FirstOrDefault(s => s.Id == id);
            if (survey == null)
            {
                return NotFound();
            }
            // Check if survey is closed (kiểm tra nếu khảo sát hết giờ => Đóng)
            if (survey.EndAt < DateTime.Now)
            {
                return View("Closed", survey); // Use a dedicated "Closed" view to inform users
            }
            return View(survey);
        }
        [HttpPost]
        public IActionResult TakeSurvey(int id, List<int> answerIds)
        {
            var survey = _context.Surveys.Find(id);
            if (survey == null)
            {
                return NotFound();
            }

            // Check if survey is closed (kiểm tra nếu khảo sát hết giờ => Đóng)
            if (survey.EndAt < DateTime.Now)
            {
                return View("Closed", survey); // Alternatively, redirect to an error or information page (tạo trang thông báo lỗi)
            }

            // Process answers and save results


            return RedirectToAction(nameof(Index));
        }
    }
}
