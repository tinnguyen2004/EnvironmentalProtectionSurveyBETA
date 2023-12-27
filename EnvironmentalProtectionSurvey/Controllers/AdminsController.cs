using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnvironmentalProtectionSurvey.Models;
using System.Net.Mail;
using System.Net;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq.Expressions;

namespace EnvironmentalProtectionSurvey.Controllers
{
    public class AdminsController : Controller
    {
        private readonly Project2Context _context;

        public AdminsController(Project2Context context)
        {
            _context = context;
        }

        //check role admin 
        public bool IsAdmin()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return false;
            }
            else
            {
                var username = HttpContext.Session.GetString("username");
                var model = _context.Users.SingleOrDefault(u => u.UserName == username && u.Role.Equals("Admin"));
                if(model != null)
                {
                    return true;

                }
                else
                {
                    return false;
                }

            }
            
        }
        // GET: Admins
        public  IActionResult Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Users");
            }
            var model =  _context.Users.Where(u=>u.Active == 1).ToList();
            return View(model);
        }

        public async Task<IActionResult> Admin(string? asearch)
        {
            var model = await _context.Users.Where(u => u.Active == 1 && u.Role == "Admin").ToListAsync();
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Users");
            }
            if (asearch == null)
            {
                return View(model);
            }
            else
            {
                var model1 = model!.Where(m => m.UserName!.Contains(asearch!) || m.Email!.Contains(asearch!) || m.NumberCode!.Contains(asearch!));
                return View(model1);
            }
        }

        public IActionResult AllUser(string? usearch)
        {
            var model = _context.Users.Where(u => u.Active == 1).ToList();
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Users");
            }
            if (usearch == null)
            {
                return View(model);
            }
            else
            {
                var model1 = model!.Where(m => m.UserName!.Contains(usearch!) || m.Email!.Contains(usearch!) || m.NumberCode!.Contains(usearch!));
                return View(model1);
            }
        }


        public async Task<IActionResult> WaitingForApproval()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Users");
            }


            if (_context.Users != null)
            {
                var activeUsers = await _context.Users.Where(u => u.Active == 0).ToListAsync();
                return View(activeUsers);
            }
            else
            {
                return Problem("Entity set 'Project2Context.Users' is null.");
            }
        }

        //gửi mail
        public void SendEmail(string toEmail, string subject ,string text)
        {
            string _smtpServer = "smtp.gmail.com";
            int _smtpPort = 587;
            string _smtpUsername = "vovansuong123456789@gmail.com";
            string _smtpPassword = "uska elgf ldya lpyy";

            using (SmtpClient smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                smtpClient.EnableSsl = true;

                MailMessage mailMessage = new MailMessage(_smtpUsername, toEmail)
                {
                    Subject = $"{subject}",
                    Body = $"{text}",
                    IsBodyHtml = true
                };

                smtpClient.Send(mailMessage);
            }
        }


        //chấp nhận user
        public async Task<IActionResult> Approve(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Users");
            }
            var model = _context.Users.SingleOrDefault(u=>u.Id == id);
            model!.Active = 1;
            await _context.SaveChangesAsync();
            //gửi mail thông báo bạn đã được duyệt
            string subject = "Congratulatory letter";
            string text = "Your account has been approved, please visit the following link to login: https://localhost:7088/Users/Login";
            SendEmail(model.Email,subject, text);
            return RedirectToAction("WaitingForApproval");
        }

        public async Task<IActionResult> Deny(int id)
        {
            var model = _context.Users.SingleOrDefault(u => u.Id == id);
            _context.Users.Remove(model!);
            await _context.SaveChangesAsync();
            string subject = "Deny letter";
            string text = "Your account has been rejected, please visit the following link to re-register: https://localhost:7088/Users/SignUp ";
            SendEmail(model.Email,subject, text);
            return RedirectToAction("WaitingForApproval");
        }


        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Users");
            }
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Users");
            }
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Password,Email,NumberCode,Class,Specification,Section,JoinDate,Role,Active,Token,ExpiryTime")] User user)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Users");
            }
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Password,Email,NumberCode,Class,Specification,Section,JoinDate,Role,Active,Token,ExpiryTime")] User user)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Users");
            }

            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Users");
            }
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Users");
            }
            if (_context.Users == null)
            {
                return Problem("Entity set 'Project2Context.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //public IActionResult CreateSurvey()
        //{
        //    // Tạo một đối tượng Survey mới để hiển thị trang tạo
        //    var newSurvey = new Survey();
        //    return View(newSurvey);
        //}

        //[HttpPost]
        //public IActionResult CreateSurvey(Survey survey)
        //{
        //    // Lưu survey vào database
        //    _context.Surveys.Add(survey);
        //    _context.SaveChanges();

        //    // Lưu questions và options vào database
        //    foreach (var question in survey.Questions)
        //    {
        //        question.SurveyId = survey.Id; // Đặt SurveyId cho mỗi câu hỏi

        //        // Lưu câu hỏi vào database
        //        _context.Questions.Add(question);
        //        _context.SaveChanges(); // Cần phải lưu để câu hỏi có ID

        //        foreach (var option in question.Options)
        //        {
        //            option.QuestionId = question.Id; // Đặt QuestionId cho mỗi option

        //            // Lưu tùy chọn vào database
        //            _context.Options.Add(option);
        //        }
        //    }

        //    _context.SaveChanges();

        //    return RedirectToAction("DetailsSurvey","Home", new { id = survey.Id });


        //    // Nếu có lỗi, hiển thị lại trang tạo với các thông báo lỗi
        //}

        //News

        public async Task<IActionResult> News()
        {
            return _context.News != null ?
                        View(await _context.News.ToListAsync()) :
                        Problem("Entity set 'SurveyProjectContext.News'  is null.");
        }

        public IActionResult CreateNews()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNews(News news, IFormFile file)
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
                    return RedirectToAction("Index");
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

        public async Task<IActionResult> EditNews(int? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNews(int id, [Bind("Id,Title,Content,Image")] News news)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        public async Task<IActionResult> DeleteNews(int? id)
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

        [HttpPost, ActionName("DeleteNews")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNews(int id)
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

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
