using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(AppDbContext db, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _env = env;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            var placedCount = await _db.JobApplications.CountAsync(a => a.Status == "Offer") + await _db.InternshipApplications.CountAsync(a => a.Status == "Completed");
            var compCount = await _db.Companies.CountAsync();
            var recentJobs = await _db.JobDescriptions.Where(j => j.IsActive && j.Status == "Approved").OrderByDescending(j => j.CreatedAt).Take(3).ToListAsync();
            var lastApp = await _db.JobApplications.Include(a => a.Student).OrderByDescending(a => a.AppliedDate).FirstOrDefaultAsync();

            var vm = new LandingPageViewModel
            {
                TotalStudents = students.Count,
                TotalPlaced = placedCount,
                TotalCompanies = compCount,
                RecentJobs = recentJobs,
                RecentApplicantName = lastApp?.Student?.FullName?.Split(' ')[0] ?? "A student"
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult SubmitJd()
        {
            return View(new JobDescriptionViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitJd(JobDescriptionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string? docPath = null;
            if (model.Document != null)
            {
                string uploads = Path.Combine(_env.WebRootPath, "uploads", "jds");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Document.FileName);
                string filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Document.CopyToAsync(stream);
                }
                docPath = "/uploads/jds/" + fileName;
            }

            var jd = new JobDescription
            {
                Title = model.Title,
                CompanyName = model.CompanyName,
                Description = model.Description,
                Requirements = model.Requirements,
                JobType = model.JobType,
                Status = "Pending",
                Deadline = model.Deadline,
                Location = model.Location,
                PackageOrStipend = model.PackageOrStipend,
                AnnualCTC = model.AnnualCTC,
                Bond = model.Bond,
                SelectionProcess = model.SelectionProcess,
                CompanyWebsite = model.CompanyWebsite,
                ContactName = model.ContactName,
                ContactEmail = model.ContactEmail,
                ContactMobile = model.ContactMobile,
                InternshipDuration = model.InternshipDuration,
                Stipend = model.Stipend,
                DateOfJoining = model.DateOfJoining,
                AdditionalDetails = model.AdditionalDetails,
                EligibleBatches = string.Join(", ", model.SelectedBatches),
                EligibleCourses = string.Join(", ", model.SelectedCourses),
                DocumentsPath = docPath,
                CreatedAt = DateTime.Now
            };

            _db.JobDescriptions.Add(jd);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Your Job Description has been submitted for review! Our T&P cell will contact you soon.";
            return RedirectToAction(nameof(Index));
        }
    }
}
