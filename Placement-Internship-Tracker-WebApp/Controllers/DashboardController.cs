using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlacementTracker.Models;
using PlacementTracker.Services;

namespace PlacementTracker.Controllers
{
    [Authorize(Roles = "Student")]
    public class DashboardController : Controller
    {
        private readonly AnalyticsService _analytics;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(AnalyticsService a, UserManager<ApplicationUser> u)
        { _analytics = a; _userManager = u; }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var vm = await _analytics.GetStudentDashboardAsync(user!.Id);
            return View(vm);
        }
    }
}
