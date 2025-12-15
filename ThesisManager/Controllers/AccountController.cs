// Controllers/AccountController.cs - UPDATED with password reset
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ThesisManager.Data;
using ThesisManager.ViewModels;

namespace ThesisManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.IsActive);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                TempData["Error"] = "البريد الإلكتروني أو كلمة المرور غير صحيحة";
                return RedirectToAction("Index", "Home");
            }

            // Check if user needs to reset password
            if (user.RequirePasswordReset)
            {
                TempData["UserId"] = user.Id;
                TempData["RequireReset"] = true;
                return RedirectToAction("ResetPassword");
            }

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id.ToString()),
                new Claim("Role", user.Role),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return RedirectToAction("Students", "Admin");
        }

        // GET: Reset Password
        public IActionResult ResetPassword()
        {
            if (TempData["RequireReset"] == null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: Reset Password
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "كلمات المرور غير متطابقة");
                return View(model);
            }

            var userId = (int?)TempData["UserId"];
            if (userId == null)
                return RedirectToAction("Index", "Home");

            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return RedirectToAction("Index", "Home");

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError("", "كلمة المرور الحالية غير صحيحة");
                TempData["UserId"] = userId;
                return View(model);
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.RequirePasswordReset = false;
            await _db.SaveChangesAsync();

            // Log user in
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id.ToString()),
                new Claim("Role", user.Role),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            TempData["Success"] = "تم تغيير كلمة المرور بنجاح";
            return RedirectToAction("Students", "Admin");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}