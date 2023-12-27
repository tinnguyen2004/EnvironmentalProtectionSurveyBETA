using EnvironmentalProtectionSurvey.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
namespace EnvironmentalProtectionSurvey.Controllers
{
    public class ChartController : Controller
    {
        private readonly Project2Context _context;
        private readonly ILogger<HomeController> _logger;
        public ChartController(Project2Context context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var counts = new { UserCount = _context.Users.Count(x => x.Role == "Staff" || x.Role == "Student"), AdminCount = _context.Users.Count(x => x.Role == "Admin"), PendingCount = _context.Users.Count(y => y.Active == 0), SurveyCount = _context.Surveys.Count(), QuestionCount = _context.Questions.Count() };
            return View(counts);
        }
        public IActionResult Charts()
        {
            return View();
        }
        [HttpPost]
        public List<object> GetList()
        {
            List<object> data = new List<object>();
            List<string?> labels = _context.Surveys.Select(x => x.Title).ToList();
            List<int?> totals = _context.Surveys.Select(t => t.UserPost).ToList();
            data.Add(labels);
            data.Add(totals);
            return data;
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
