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
    public class NewsController : Controller
    {
        private readonly Project2Context _context;

        public NewsController(Project2Context context)
        {
            _context = context;
        }

        // GET: News1
        public async Task<IActionResult> Index()
        {
            return _context.News != null ?
                        View(await _context.News.ToListAsync()) :
                        Problem("Entity set 'Project2Context.News'  is null.");
        }

        // GET: News1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(News news, IFormFile file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string path = Path.Combine("wwwroot/Images", file.FileName);
                    var stream = new FileStream(path, FileMode.Create);
                    file.CopyToAsync(stream);

                    news.Image = "/Images/" + file.FileName;
                    _context.Add(news);
                    _context.SaveChanges();
                    // Display success alert using SweetAlert2
                    return Json(new { success = true });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Fail");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                throw;
            }
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: News1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,Image")] News news,IFormFile file)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                        string path = Path.Combine("wwwroot/Images", file.FileName);
                        var stream = new FileStream(path, FileMode.Create);
                        file.CopyToAsync(stream);

                        news.Image = "/Images/" + file.FileName;
                        _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
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
            return View(news);
        }

        // GET: News1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.News == null)
            {
                return Problem("Entity set 'SurveyProjectContext.News'  is null.");
            }
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return (_context.News?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
