using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;
using PlacementTracker.Services;

namespace PlacementTracker.Controllers
{
    [Authorize(Roles = "Student")]
    public class DashboardController : Controller
    {
        private readonly AnalyticsService _analytics;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;

        public DashboardController(AnalyticsService a, UserManager<ApplicationUser> u, AppDbContext db)
        { _analytics = a; _userManager = u; _db = db; }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var vm = await _analytics.GetStudentDashboardAsync(user!.Id);

            // Placement Lock check
            var selectedPlacement = await _db.JobApplications
                .FirstOrDefaultAsync(a => a.StudentId == user.Id && a.Status == "Selected" && a.IsActive);
            ViewBag.IsPlacementLocked = selectedPlacement != null;
            ViewBag.SelectedCompany = selectedPlacement?.CompanyName;

            return View(vm);
        }
    }
}
