using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;
using PlacementTracker.ViewModels;
using PlacementTracker.Services;

namespace PlacementTracker.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NotificationService _notificationSvc;

        public FacultyController(AppDbContext db, UserManager<ApplicationUser> u, NotificationService n)
        { _db = db; _userManager = u; _notificationSvc = n; }

        private async Task<string> GetFacultyId()
            => (await _userManager.GetUserAsync(User))!.Id;

        public async Task<IActionResult> Index()
        {
            var facultyId = await GetFacultyId();
            var studentIds = await _db.FacultyStudentMappings
                .Where(m => m.FacultyId == facultyId)
                .Select(m => m.StudentId).ToListAsync();

            var students = await _db.Users
                .Where(u => studentIds.Contains(u.Id)).ToListAsync();

            var items = new List<StudentProgressItem>();
            foreach (var s in students)
            {
                var apps = await _db.JobApplications
                    .Where(a => a.StudentId == s.Id && a.IsActive).ToListAsync();
                var interns = await _db.InternshipApplications
                    .Where(a => a.StudentId == s.Id && a.IsActive).ToListAsync();

                items.Add(new StudentProgressItem {
                    Student             = s,
                    TotalApps           = apps.Count,
                    Offers              = apps.Count(a => a.Status == "Offer"),
                    Interviews          = apps.Count(a => a.Status == "Interview"),
                    LatestStatus        = apps.OrderByDescending(a => a.UpdatedAt)
                                             .FirstOrDefault()?.Status ?? "—",
                    TotalInternships    = interns.Count,
                    InternshipSelected  = interns.Count(a => a.Status == "Selected" || a.Status == "Completed"),
                    HasFullTimeOffer     = interns.Any(a => a.IsFullTimeOffered),
                    HasOngoingInternship = interns.Any(a => a.IsOngoing)
                });
            }

            var upcomingPlacement = await _db.InterviewSchedules
                .Include(i => i.JobApplication).ThenInclude(a => a!.Student)
                .Where(i => studentIds.Contains(i.JobApplication!.StudentId)
                    && i.InterviewDate >= DateTime.Today)
                .OrderBy(i => i.InterviewDate).Take(10).ToListAsync();

            var upcomingInternship = await _db.InternshipInterviews
                .Include(i => i.InternshipApplication).ThenInclude(a => a!.Student)
                .Where(i => studentIds.Contains(i.InternshipApplication!.StudentId)
                    && i.InterviewDate >= DateTime.Today)
                .OrderBy(i => i.InterviewDate).Take(10).ToListAsync();

            return View(new FacultyDashboardViewModel {
                Students                   = items,
                UpcomingInterviews         = upcomingPlacement,
                UpcomingInternshipInterviews = upcomingInternship,
                TotalStudents              = students.Count,
                TotalApplications          = items.Sum(i => i.TotalApps),
                TotalOffers                = items.Sum(i => i.Offers),
                TotalInternships           = items.Sum(i => i.TotalInternships),
                TotalInternshipSelected    = items.Sum(i => i.InternshipSelected),
                TotalFullTimeOffered       = items.Count(i => i.HasFullTimeOffer)
            });
        }

        public async Task<IActionResult> StudentDetails(string studentId)
        {
            var facultyId = await GetFacultyId();
            if (!await _db.FacultyStudentMappings.AnyAsync(
                m => m.FacultyId == facultyId && m.StudentId == studentId))
                return Forbid();

            var student = await _db.Users.FindAsync(studentId);
            var placements = await _db.JobApplications
                .Include(a => a.Remarks).ThenInclude(r => r.Faculty)
                .Include(a => a.Interviews)
                .Where(a => a.StudentId == studentId && a.IsActive).ToListAsync();
            var internships = await _db.InternshipApplications
                .Include(a => a.Remarks).ThenInclude(r => r.Faculty)
                .Include(a => a.Interviews)
                .Where(a => a.StudentId == studentId && a.IsActive).ToListAsync();

            ViewBag.Student    = student;
            ViewBag.Placements = placements;
            ViewBag.Internships= internships;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRemark(string studentId, int? appId,
            int? internshipId, string comment)
        {
            var facultyId = await GetFacultyId();
            _db.FacultyRemarks.Add(new FacultyRemark {
                FacultyId              = facultyId,
                JobApplicationId       = appId,
                InternshipApplicationId= internshipId,
                Comment                = comment
            });
            await _db.SaveChangesAsync();
            
            var faculty = await _userManager.GetUserAsync(User);
            await _notificationSvc.SendAsync(studentId, $"Faculty {faculty?.FullName} added a remark on your application.");
            
            TempData["Success"] = "Remark added!";
            return RedirectToAction(nameof(StudentDetails), new { studentId });
        }
    }
}
