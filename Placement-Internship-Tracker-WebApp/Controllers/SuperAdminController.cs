using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;

namespace PlacementTracker.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SuperAdminController(AppDbContext db,
            UserManager<ApplicationUser> u, RoleManager<IdentityRole> r)
        { _db = db; _userManager = u; _roleManager = r; }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers        = await _db.Users.CountAsync();
            ViewBag.TotalPlacements   = await _db.JobApplications.CountAsync(a => a.IsActive);
            ViewBag.TotalInternships  = await _db.InternshipApplications.CountAsync(a => a.IsActive);
            ViewBag.TotalOffers       = await _db.JobApplications.CountAsync(a => a.Status == "Offer" && a.IsActive);
            ViewBag.TotalPPO          = await _db.InternshipApplications.CountAsync(a => a.IsPPOConverted && a.IsActive);
            ViewBag.TotalCompanies    = await _db.Companies.CountAsync(c => c.IsActive);
            ViewBag.AllUsers          = await _db.Users.OrderBy(u => u.FullName).ToListAsync();
            return View();
        }
    }
}
