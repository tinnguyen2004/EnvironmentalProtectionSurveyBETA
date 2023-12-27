using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EnvironmentalProtectionSurvey.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using BCrypt.Net;
namespace EnvironmentalProtectionSurvey.Controllers
{
    public class UsersController : Controller
    {
        private readonly Project2Context _context;

        public UsersController(Project2Context context)
        {
            _context = context;
        }


        //Create token 
        public string GenerateToken()
        {
            // Thư viện JWT thực tế sẽ được sử dụng ở đây để tạo token 
            return Guid.NewGuid().ToString();
        }

        //Send mail 
        public void SendVerificationEmail(string toEmail, string token)
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
                    Subject = "Verification Email",
                    Body = $@"
                <html>
                    <head>
                        <style>
                            body {{
                                font-family: 'Arial', sans-serif;
                                margin: 0;
                                padding: 0;
                            }}
                            .container {{
                                width: 80%;
                                margin: auto;
                                overflow: hidden;
                            }}
                            header {{
                                background: #fff;
                                padding: 1em 0;
                            }}
                            header img {{
                                height: 50px;
                            }}
                            .main {{
                                padding: 20px 0;
                            }}
                            .verification-link {{
                                display: inline-block;
                                padding: 10px 20px;
                                font-size: 16px;
                                background-color: #3498db;
                                color: #fff;
                                text-decoration: none;
                                border-radius: 5px;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container main'>
                            <p>Dear user,</p>
                            <p>Thank you for signing up. Please click the link below to verify your account:</p>
                            <a href='https://localhost:7088/Users/VerifyEmail?token={token}' class='verification-link'>Verify Your Account</a>
                            <p>If you did not sign up, you can safely ignore this email.</p>
                            <p>Best regards,<br>Your Company Name</p>
                        </div>
                    </body>
                </html>",
                    IsBodyHtml = true
                };

                smtpClient.Send(mailMessage);
            }
        }

        public IActionResult VerifyEmail(string token)
        {
            var user = _context.Users.FirstOrDefault(u => u.Token == token && u.ExpiryTime > DateTime.UtcNow);
            if (user != null)
            {

                user.Active = 0;
                user.Token = null; // Xóa token sau khi tài khoản đã được kích hoạt
                user.ExpiryTime = null;
                _context.SaveChanges();
                TempData["successful"] = "Your account has been successfully activated. Please wait for admin to review your account";
                return RedirectToAction("Login");
            }
            else
            {
                try
                {
                    var model = _context.Users.FirstOrDefault(u => u.Token == token);
                    _context.Users.Remove(model!);
                    _context.SaveChanges();
                    ViewBag.msg = "Invalid or expired token, Please sign up again";
                }
                catch (Exception ex)
                {
                    ViewBag.msg = ex.Message;

                }
            }

            return View();
        }

        public IActionResult VerifyEmailFogetPassword(string token)
        {
            var user = _context.Users.FirstOrDefault(u => u.Token == token && u.ExpiryTime > DateTime.UtcNow);
            if (user != null)
            {

                user.Token = null; // Xóa token sau khi tài khoản đã được kích hoạt
                user.ExpiryTime = null;
                _context.SaveChanges();
                TempData["userID"] = user.Id;
                return RedirectToAction("ChangePassword");
            }
            else
            {
                try
                {
                    ViewBag.msg = "Invalid or expired token, Please try again";
                    return View();

                }
                catch (Exception ex)
                {
                    ViewBag.msg = ex.Message;
                    return View();

                }
            }

        }

        public IActionResult ChangePassword()
        {
            if (TempData.ContainsKey("userID"))
            {
                var userId = (int)TempData["userID"]!;
                var model = _context.Users.FirstOrDefault(u => u.Id == userId);

                if (model != null)
                {
                    return View(model);
                }
            }
            ViewBag.msg = "Not fonnd user";
            return View();
        }

        private bool IsPasswordValid(string password)
        {
            // Thực hiện kiểm tra yêu cầu về password
            // Password phải có ít nhất 8 ký tự, một chữ cái in hoa, một chữ số, và một ký tự đặc biệt
            // Bạn có thể điều chỉnh biểu thức chính quy tùy theo yêu cầu cụ thể của bạn
            var regex = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return regex.IsMatch(password);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string email, string password, string confirmPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                if (password != confirmPassword)
                {
                    ViewBag.confirmPassword = "New password and confirm password must be the same";
                    return View(user);
                }

                if (!IsPasswordValid(password))
                {
                    ViewBag.msg = "Password must be at least 8 characters long and contain at least one uppercase letter, one digit, and one special character.";
                    return View(user);
                }

                user.Password = password;
                _context.SaveChanges();

                TempData["successful"] = "Your password has been successfully changed.";
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.msg = "Invalid";
                return View();
            }
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgetPassword(string email)
        {
            var model = _context.Users.SingleOrDefault(u => u.Email == email);
            if (model == null)
            {
                ViewBag.err = "Email does not exist";
                return View();
            }
            else
            {
                var token = GenerateToken();
                model.Token = token;
                model.ExpiryTime = DateTime.UtcNow.AddMinutes(15);
                _context.SaveChanges();
                //gui mail
                string _smtpServer = "smtp.gmail.com";
                int _smtpPort = 587;
                string _smtpUsername = "vovansuong123456789@gmail.com";
                string _smtpPassword = "uska elgf ldya lpyy";

                using (SmtpClient smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage(_smtpUsername, model.Email)
                    {
                        Subject = "Forget Password",
                        Body = $"https://localhost:7088/Users/VerifyEmailFogetPassword?token={token}",
                        IsBodyHtml = true
                    };

                    smtpClient.Send(mailMessage);
                }
                ViewBag.msg = "Please check your email";
                return View();

            }


        }

        //Sign up
        public IActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(User user, string confirmPassword)
        {
            if (ModelState.IsValid)
            {
                //kiểm tra username có tồn tại hay chưa 
                var username = _context.Users.SingleOrDefault(u => u.UserName == user.UserName);
                if (username != null)
                {
                    ViewBag.errUsername = "Username already exists in the system";
                    return View(user);
                }
                //kiểm tra email có tồn tại hay chưa
                var email = _context.Users.SingleOrDefault(u => u.Email == user.Email);
                if (email != null)
                {
                    ViewBag.errEmail = "Email already exists in the system";
                    return View(user);
                }

                if (username == null)
                {
                    var token = GenerateToken();
                    //xác thực mail
                    user.Token = token;
                    user.ExpiryTime = DateTime.UtcNow.AddMinutes(15); //Token hết hạn sau 15p
                    if (user.Password != confirmPassword)
                    {
                        ViewBag.confirmPassword = "Password and ConfirmPassword must be the same";
                        return View(user);
                    }

                    // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                    // Kiểm tra NumberCode bắt đầu bằng "student" hoặc "teacher"
                    if (user.NumberCode == null || !(user.NumberCode.StartsWith("student", StringComparison.OrdinalIgnoreCase) || user.NumberCode.StartsWith("teacher", StringComparison.OrdinalIgnoreCase)))
                    {
                        ViewBag.errNumberCode = "NumberCode must start with 'student' or 'teacher'";
                        return View(user);
                    }
                    //kiểm tra NumberCode có tồn tại hay chưa 
                    var numberCode = _context.Users.SingleOrDefault(u => u.NumberCode == user.NumberCode);
                    if (numberCode != null)
                    {
                        ViewBag.errNumberCode = "numberCode already exists in the system";
                        return View(user);
                    }
                    //tự động cài role dựa theo numberCode
                    if (user.NumberCode.StartsWith("Student", StringComparison.OrdinalIgnoreCase))
                    {
                        user.Role = "Student";
                    }
                    if (user.NumberCode.StartsWith("Teacher", StringComparison.OrdinalIgnoreCase))
                    {
                        user.Role = "Teacher";
                    }


                    //chuyển active = 2 vào trạng thái đợi xác thực mail
                    user.Active = 2;
                    _context.Add(user);
                    await _context.SaveChangesAsync();




                    try
                    {
                        //gửi mail xác thực 
                        // code gửi email
                        SendVerificationEmail(user.Email, token);

                    }
                    catch (Exception ex)
                    {
                        ViewBag.msg = ex.Message;
                        return View();
                    }
                    TempData["sendMail"] = "Please check your email and verify your account";
                    return RedirectToAction("Login");
                }
            }
            return View(user);
        }

        //Login

        [HttpGet]
        public IActionResult Login()
        {
            if (TempData.ContainsKey("successful"))
            {
                ViewBag.successful = TempData["successful"];
            }
            if (TempData.ContainsKey("sendMail"))
            {
                ViewBag.sendMail = TempData["sendMail"];
            }
            return View();
        }

        //Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var model = _context.Users.FirstOrDefault(u => u.UserName == username);
            if (model != null && BCrypt.Net.BCrypt.Verify(password, model.Password))
            {
                HttpContext.Session.SetString("username", model.UserName!.ToString());
                if (model.Active == 0)
                {
                    ViewBag.notActive = "Please wait for admin to review your account";
                    return View();
                }
                if (model.Active == 2)
                {
                    ViewBag.notActive = "Please verify your email";
                    return View();
                }
                if (model.Role == "Admin")
                {

                    return RedirectToAction("Index", "Admins");

                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.msg = "Wrong username or password";
            }
            return View();
        }

        //Logout 
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");

            return RedirectToAction("Index", "Home");
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var model = await _context.Users.ToListAsync();
            return View(model);
        }

        // GET: Users/Details/5
        public IActionResult Details()
        {
            var username = HttpContext.Session.GetString("username");
            var model = _context.Users.SingleOrDefault(u => u.UserName == username);
            if (model != null)
            {
                return View(model);
            }
            return NotFound();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidNumberCode(string numberCode)
        {

            // Number code should start with "student" or "teacher"
            if (numberCode == null || !(numberCode.StartsWith("student", StringComparison.OrdinalIgnoreCase) || numberCode.StartsWith("teacher", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            return true;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(User user, string btn, string password, string newPassword)
        {

            if (btn == "Save")
            {
                try
                {
                    var model = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
                    if (!IsValidEmail(user.Email))
                    {
                        ModelState.AddModelError("Email", "Invalid email format");
                        return View(user);
                    }

                    if (!IsValidNumberCode(user.NumberCode))
                    {
                        ModelState.AddModelError("NumberCode", "Invalid number code format");
                        return View(user);
                    }

                    if (model != null)
                    {

                        model!.Email = user.Email;
                        model.NumberCode = user.NumberCode;
                        model.Class = user.Class;
                        model.Specification = user.Specification;
                        model.Section = user.Section;

                        //await  _context.SaveChangesAsync(user);
                        await _context.SaveChangesAsync();
                        ViewBag.updated = "Information has been updated";
                        return View(model);

                    }
                    else
                    {
                        ViewBag.thatbai = "not found...";
                    }
                }
                catch (Exception)
                {
                    ViewBag.msg = "Fails";
                }
            }
            else
            {
                var model = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);

                if (model != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(password, model.Password))
                    {
                        if (IsPasswordValid(newPassword))
                        {
                            model.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                            ViewBag.updatedPass = "Password has been updated";
                            _context.SaveChanges();
                            return View(user);
                        }
                        else
                        {
                            ViewBag.err = "Password must be at least 8 characters long and contain at least one uppercase letter, one digit, and one special character";
                            return View(user);

                        }
                    }
                    else
                    {
                        ViewBag.failsPass = "Incorrect current password";
                    }
                }
            }

            return View(user);
        }


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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



        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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


        //surveyParticipated
        public IActionResult SurveyParticipationHistory()
        {
            var username = HttpContext.Session.GetString("username");

            if (username != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserName == username);

                if (user != null)
                {
                    var filledSurveys = _context.FilledSurveys
                        .Include(fs => fs.Survey)
                        .Include(fs => fs.Option)
                        .Where(fs => fs.UserId == user.Id)
                        .GroupBy(fs => fs.SurveyId)  // Group by SurveyId
                        .Select(group => group.First())
                        .ToList();

                    return View(filledSurveys);
                }
            }

            return View(new List<FilledSurvey>());
        }

        public async Task<IActionResult> DetailsSurveyParticipationHistory(int surveyId)
        {
            var username = HttpContext.Session.GetString("username");

            if (username != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.UserName == username);

                if (user != null)
                {
                    var filledSurveyDetails = await (
                        from fs in _context.FilledSurveys
                        join o in _context.Options on fs.OptionId equals o.Id
                        join q in _context.Questions on o.QuestionId equals q.Id
                        join s in _context.Surveys on q.SurveyId equals s.Id
                        where fs.UserId == user.Id && fs.SurveyId == surveyId
                        select new FilledSurveyDetails
                        {
                            QuestionId = q.Id,
                            FilledSurvey = fs,
                            SelectedOptions = _context.Options
                                .Where(opt => opt.QuestionId == q.Id && opt.Id == fs.OptionId)
                                .ToList(),
                            Question = q,
                            Survey = s
                        }
                    ).ToListAsync();

                    if (filledSurveyDetails == null)
                    {
                        return NotFound();
                    }

                    return View(filledSurveyDetails);
                }
            }

            return NotFound();
        }
        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
