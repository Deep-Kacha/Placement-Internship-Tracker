using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;
using PlacementTracker.Services;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AnalyticsService _analytics;
        private readonly InternshipService _internshipSvc;
        private readonly NotificationService _notificationSvc;

        public AdminController(AppDbContext db,
            UserManager<ApplicationUser> u, AnalyticsService a, InternshipService i, NotificationService n)
        { _db = db; _userManager = u; _analytics = a; _internshipSvc = i; _notificationSvc = n; }

        public async Task<IActionResult> Index()
            => View(await _analytics.GetAdminDashboardAsync());

        public async Task<IActionResult> Applications(
            string? status, string? department, string? company)
        {
            var q = _db.JobApplications
                .Include(a => a.Student)
                .Where(a => a.IsActive).AsQueryable();

            if (!string.IsNullOrEmpty(status))    q = q.Where(a => a.Status == status);
            if (!string.IsNullOrEmpty(company))   q = q.Where(a => a.CompanyName.Contains(company));
            if (!string.IsNullOrEmpty(department)) q = q.Where(a => a.Student!.Department == department);

            ViewBag.StatusList  = ApplicationFormViewModel.StatusList;
            ViewBag.Departments = await _db.Users
                .Where(u => u.Department != null).Select(u => u.Department!).Distinct().ToListAsync();
            return View(await q.OrderByDescending(a => a.UpdatedAt).ToListAsync());
        }

        public async Task<IActionResult> Internships(
            string? status, string? department, string? company, string? internshipType)
        {
            var list = await _internshipSvc.GetAllInternshipsAsync(status, department, company, internshipType);

            ViewBag.StatusList  = InternshipFormViewModel.StatusList;
            ViewBag.TypeList    = InternshipFormViewModel.TypeList;
            ViewBag.Departments = await _db.Users
                .Where(u => u.Department != null).Select(u => u.Department!).Distinct().ToListAsync();
            ViewBag.Filter_Status        = status;
            ViewBag.Filter_Company       = company;
            ViewBag.Filter_Department    = department;
            ViewBag.Filter_InternshipType= internshipType;
            return View(list);
        }

        public async Task<IActionResult> ExportInternshipsExcel()
        {
            var list = await _internshipSvc.GetAllInternshipsAsync();
            using var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.Worksheets.Add("All Internships");
            string[] headers = {
                "Student","Roll No","Department","Company","Role",
                "Type","Mode","Status","Stipend","Full-time Offered","FTO Package",
                "Certificate","Start Date","End Date"
            };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i+1).Value = headers[i];
            int row = 2;
            foreach (var a in list)
            {
                ws.Cell(row, 1).Value  = a.Student?.FullName ?? "-";
                ws.Cell(row, 2).Value  = a.Student?.CollegeRollNo ?? "-";
                ws.Cell(row, 3).Value  = a.Student?.Department ?? "-";
                ws.Cell(row, 4).Value  = a.CompanyName;
                ws.Cell(row, 5).Value  = a.Role;
                ws.Cell(row, 6).Value  = a.InternshipType;
                ws.Cell(row, 7).Value  = a.WorkMode;
                ws.Cell(row, 8).Value  = a.Status;
                ws.Cell(row, 9).Value  = a.Stipend != null ? $"₹{a.Stipend}" : "-";
                ws.Cell(row,10).Value  = a.IsFullTimeOffered ? "Yes" : "No";
                ws.Cell(row,11).Value  = a.FullTimePackage ?? "-";
                ws.Cell(row,12).Value  = a.CertificateReceived ? "Yes" : "No";
                ws.Cell(row,13).Value  = a.StartDate?.ToString("dd-MM-yyyy") ?? "-";
                ws.Cell(row,14).Value  = a.EndDate?.ToString("dd-MM-yyyy") ?? "-";
                row++;
            }
            ws.Row(1).Style.Font.Bold = true;
            ws.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "InternshipReport.xlsx");
        }

        public async Task<IActionResult> Users()
        {
            ViewBag.Students = (await _userManager.GetUsersInRoleAsync("Student")).OrderBy(s => s.FullName).ToList();
            ViewBag.Faculty  = (await _userManager.GetUsersInRoleAsync("Faculty")).OrderBy(f => f.FullName).ToList();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveStudent(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user != null) {
                user.IsApproved = true;
                await _userManager.UpdateAsync(user);
                await _notificationSvc.SendAsync(user.Id, "Congratulations! Your account has been approved by the Administrator.");
                TempData["Success"] = "Student approved!";
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpGet] public IActionResult CreateStudent()
            => View(new RegisterViewModel { Role = "Student" });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(RegisterViewModel model)
        {
            model.Role = "Student";
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser {
                UserName = model.Email, Email = model.Email,
                FullName = model.FullName, Department = model.Department,
                CollegeRollNo = model.CollegeRollNo, Phone = model.Phone,
                EmailConfirmed = true
            };
            var r = await _userManager.CreateAsync(user, model.Password);
            if (r.Succeeded)
            { await _userManager.AddToRoleAsync(user, "Student");
              TempData["Success"] = "Student created!"; return RedirectToAction(nameof(Users)); }
            foreach (var e in r.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        [HttpGet] public IActionResult CreateFaculty()
            => View(new RegisterViewModel { Role = "Faculty" });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFaculty(RegisterViewModel model)
        {
            model.Role = "Faculty";
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser {
                UserName = model.Email, Email = model.Email,
                FullName = model.FullName, Department = model.Department,
                EmailConfirmed = true
            };
            var r = await _userManager.CreateAsync(user, model.Password);
            if (r.Succeeded)
            { await _userManager.AddToRoleAsync(user, "Faculty");
              TempData["Success"] = "Faculty created!"; return RedirectToAction(nameof(Users)); }
            foreach (var e in r.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null) { user.IsActive = !user.IsActive; await _userManager.UpdateAsync(user); }
            TempData["Success"] = "User status updated.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStudentToFaculty(string facultyId, string studentId)
        {
            if (!_db.FacultyStudentMappings.Any(m => m.FacultyId == facultyId && m.StudentId == studentId))
            {
                _db.FacultyStudentMappings.Add(new FacultyStudentMapping {
                    FacultyId = facultyId, StudentId = studentId });
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "Student assigned!";
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> ExportExcel()
        {
            using var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.Worksheets.Add("All Placements");
            string[] headers = { "Student","Roll No","Department","Company","Role","Status","Applied Date","Package","Location" };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i+1).Value = headers[i];
            var apps = await _db.JobApplications
                .Include(a => a.Student).Where(a => a.IsActive)
                .OrderByDescending(a => a.UpdatedAt).ToListAsync();
            int row = 2;
            foreach (var a in apps)
            {
                ws.Cell(row,1).Value = a.Student?.FullName ?? "-";
                ws.Cell(row,2).Value = a.Student?.CollegeRollNo ?? "-";
                ws.Cell(row,3).Value = a.Student?.Department ?? "-";
                ws.Cell(row,4).Value = a.CompanyName;
                ws.Cell(row,5).Value = a.Role;
                ws.Cell(row,6).Value = a.Status;
                ws.Cell(row,7).Value = a.AppliedDate.ToString("dd-MM-yyyy");
                ws.Cell(row,8).Value = a.Package ?? "-";
                ws.Cell(row,9).Value = a.Location ?? "-";
                row++;
            }
            ws.Row(1).Style.Font.Bold = true;
            ws.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "PlacementReport.xlsx");
        }

        public async Task<IActionResult> Companies()
            => View(await _db.Companies.OrderBy(c => c.Name).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCompany(string name, string? location,
            string? industry, string? website, bool offersInternships, bool offersPlacement)
        {
            _db.Companies.Add(new Company {
                Name = name, Location = location, Industry = industry,
                Website = website, OffersInternships = offersInternships,
                OffersPlacement = offersPlacement });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Company added!";
            return RedirectToAction(nameof(Companies));
        }

        public async Task<IActionResult> JobDescriptions()
        {
            var jds = await _db.JobDescriptions
                .Include(j => j.Recruiter)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();
            return View(jds);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateJDStatus(int id, string status)
        {
            var jd = await _db.JobDescriptions.FindAsync(id);
            if(jd != null) {
                jd.Status = status;
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Job Description {status}!";
            }
            return RedirectToAction(nameof(JobDescriptions));
        }

        public async Task<IActionResult> JobDescriptionDetails(int id)
        {
            var jd = await _db.JobDescriptions
                .Include(j => j.JobApplications)
                    .ThenInclude(a => a.Student)
                .Include(j => j.InternshipApplications)
                    .ThenInclude(a => a.Student)
                .FirstOrDefaultAsync(j => j.Id == id);
            
            if (jd == null) return NotFound();
            return View(jd);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdateApplicantStatus(int jdId, string[] studentIds, string newStatus)
        {
            var jd = await _db.JobDescriptions.FindAsync(jdId);
            if (jd == null) return NotFound();
            if (studentIds == null || !studentIds.Any()) return RedirectToAction(nameof(JobDescriptionDetails), new { id = jdId });

            if (jd.JobType == "Internship")
            {
                var apps = await _db.InternshipApplications
                    .Where(a => a.JobDescriptionId == jdId && studentIds.Contains(a.StudentId))
                    .ToListAsync();
                foreach(var a in apps) {
                    a.Status = newStatus;
                    await _notificationSvc.SendAsync(a.StudentId, $"Update: Your internship status for {jd.CompanyName} - {jd.Title} has been changed to {newStatus}.", $"/Internships/Details/{a.Id}");
                }
            }
            else
            {
                var apps = await _db.JobApplications
                    .Where(a => a.JobDescriptionId == jdId && studentIds.Contains(a.StudentId))
                    .ToListAsync();

                // Placement Rule: If Selected, lock this student from applying to new opportunities
                foreach(var a in apps) {
                    a.Status = newStatus;
                    if (newStatus == "Selected")
                    {
                        await _notificationSvc.SendAsync(a.StudentId, $"🎉 Congratulations! You have been Selected for {jd.CompanyName} - {jd.Title}. Your application portal is now locked.", $"/Applications/Details/{a.Id}");
                    }
                    else
                    {
                        await _notificationSvc.SendAsync(a.StudentId, $"Update: Your application status for {jd.CompanyName} - {jd.Title} has been changed to {newStatus}.", $"/Applications/Details/{a.Id}");
                    }
                }
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = $"Updated {studentIds.Length} applicants to {newStatus}.";
            return RedirectToAction(nameof(JobDescriptionDetails), new { id = jdId });
        }
    }
}
