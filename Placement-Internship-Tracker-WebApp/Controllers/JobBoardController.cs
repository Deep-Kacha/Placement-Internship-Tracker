using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;

namespace PlacementTracker.Controllers
{
    [Authorize(Roles = "Student")]
    public class JobBoardController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobBoardController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? type)
        {
            var query = _db.JobDescriptions.Where(j => j.Status == "Approved" && j.IsActive);
            if (!string.IsNullOrEmpty(type)) query = query.Where(j => j.JobType == type);
            var jds = await query.OrderByDescending(j => j.CreatedAt).ToListAsync();
            
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var appliedJobIds = await _db.JobApplications.Where(a => a.StudentId == user.Id && a.JobDescriptionId != null).Select(a => a.JobDescriptionId).ToListAsync();
            var appliedInternIds = await _db.InternshipApplications.Where(a => a.StudentId == user.Id && a.JobDescriptionId != null).Select(a => a.JobDescriptionId).ToListAsync();
            
            ViewBag.AppliedJobIds = new HashSet<int?>(appliedJobIds.Concat(appliedInternIds));
            return View(jds);
        }

        public async Task<IActionResult> Details(int id)
        {
            var jd = await _db.JobDescriptions.Include(j => j.Recruiter).FirstOrDefaultAsync(j => j.Id == id);
            if(jd == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            bool applied = false;
            if(jd.JobType == "Internship") applied = await _db.InternshipApplications.AnyAsync(a => a.StudentId == user.Id && a.JobDescriptionId == id);
            else applied = await _db.JobApplications.AnyAsync(a => a.StudentId == user.Id && a.JobDescriptionId == id);

            ViewBag.HasApplied = applied;
            return View(jd);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int id)
        {
            var jd = await _db.JobDescriptions.FindAsync(id);
            if(jd == null || jd.Status != "Approved") return NotFound();
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if(jd.JobType == "Internship") {
                if(await _db.InternshipApplications.AnyAsync(a => a.StudentId == user.Id && a.JobDescriptionId == id)) {
                    TempData["Error"] = "Already applied!"; return RedirectToAction(nameof(Details), new { id });
                }
                _db.InternshipApplications.Add(new InternshipApplication {
                    StudentId = user.Id, JobDescriptionId = jd.Id,
                    CompanyName = jd.CompanyName, Role = jd.Title,
                    InternshipType = "On-Campus", WorkMode = "In-Person", Status = "Applied",
                    AppliedDate = DateTime.Now, UpdatedAt = DateTime.Now
                });
            } else {
                if(await _db.JobApplications.AnyAsync(a => a.StudentId == user.Id && a.JobDescriptionId == id)) {
                    TempData["Error"] = "Already applied!"; return RedirectToAction(nameof(Details), new { id });
                }
                _db.JobApplications.Add(new JobApplication {
                    StudentId = user.Id, JobDescriptionId = jd.Id,
                    CompanyName = jd.CompanyName, Role = jd.Title, Status = "Applied",
                    AppliedDate = DateTime.Now, UpdatedAt = DateTime.Now
                });
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Successfully applied to " + jd.Title;
            return RedirectToAction(nameof(Index));
        }
    }
}
