using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Controllers
{
    [Authorize(Roles = "Recruiter")]
    public class RecruiterController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RecruiterController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null) return RedirectToAction("Login", "Account");

            var myJobs = await _context.JobDescriptions
                .Where(j => j.RecruiterId == user.Id)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
            return View(myJobs);
        }

        [HttpGet]
        public IActionResult CreateJD()
        {
            return View(new JobDescriptionViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJD(JobDescriptionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if(user == null) return RedirectToAction("Login", "Account");

            var jd = new JobDescription
            {
                Title = model.Title,
                CompanyName = model.CompanyName,
                Description = model.Description,
                Requirements = model.Requirements,
                JobType = model.JobType,
                Status = "Pending", // Requires Admin Approval
                Deadline = model.Deadline,
                Location = model.Location,
                PackageOrStipend = model.PackageOrStipend,
                RecruiterId = user.Id
            };

            _context.JobDescriptions.Add(jd);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Job Description submitted successfully! Awaiting Admin approval.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null) return RedirectToAction("Login", "Account");

            var jd = await _context.JobDescriptions
                .Include(j => j.JobApplications)
                    .ThenInclude(a => a.Student)
                .Include(j => j.InternshipApplications)
                    .ThenInclude(a => a.Student)
                .FirstOrDefaultAsync(j => j.Id == id && j.RecruiterId == user.Id);

            if (jd == null) return NotFound();
            return View(jd);
        }
    }
}
