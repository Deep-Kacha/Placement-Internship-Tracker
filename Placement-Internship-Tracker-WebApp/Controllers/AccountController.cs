using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlacementTracker.Models;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Services.NotificationService _notificationService;

        public AccountController(UserManager<ApplicationUser> u,
            SignInManager<ApplicationUser> s, RoleManager<IdentityRole> r,
            Services.NotificationService n)
        { _userManager = u; _signInManager = s; _roleManager = r; _notificationService = n; }

        [HttpGet] public IActionResult Login() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            { ModelState.AddModelError("", "Invalid credentials."); return View(model); }

            var roles = await _userManager.GetRolesAsync(user);
            var role  = roles.FirstOrDefault();

            if (role == "Student" && !user.IsApproved)
            { ModelState.AddModelError("", "Your account is pending Admin approval."); return View(model); }

            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe, false);

            if (!result.Succeeded)
            { ModelState.AddModelError("", "Invalid credentials."); return View(model); }

            return role switch {
                "SuperAdmin" => RedirectToAction("Index", "SuperAdmin"),
                "Admin"      => RedirectToAction("Index", "Admin"),
                "Faculty"    => RedirectToAction("Index", "Faculty"),
                "Recruiter"  => RedirectToAction("Index", "Recruiter"),
                _            => RedirectToAction("Index", "Dashboard")
            };
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        { await _signInManager.SignOutAsync(); return RedirectToAction("Login"); }

        [HttpGet] public IActionResult Register() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser {
                UserName = model.Email, Email = model.Email,
                FullName = model.FullName, Department = model.Department,
                CollegeRollNo = model.CollegeRollNo, Phone = model.Phone,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                TempData["Success"] = "Account created successfully!";
                return RedirectToAction("Login");
            }
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        public IActionResult AccessDenied() => View();

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var model = new ProfileViewModel {
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Department = user.Department,
                CollegeRollNo = user.CollegeRollNo
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            user.FullName = model.FullName;
            user.Phone = model.Phone;
            user.Department = model.Department;
            user.CollegeRollNo = model.CollegeRollNo;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Notifications()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var notifications = await _notificationService.GetNotificationsAsync(user.Id);
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkNotificationRead(int id)
        {
            await _notificationService.MarkReadAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            await _notificationService.DeleteAsync(id);
            return RedirectToAction("Notifications");
        }
    }
}
