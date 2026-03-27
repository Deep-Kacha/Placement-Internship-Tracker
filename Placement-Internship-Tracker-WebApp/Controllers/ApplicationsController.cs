using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlacementTracker.Models;
using PlacementTracker.Services;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Controllers
{
    [Authorize(Roles = "Student")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationService _svc;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly NotificationService _notificationSvc;

        public ApplicationsController(ApplicationService s, UserManager<ApplicationUser> u, NotificationService n)
        { _svc = s; _userManager = u; _notificationSvc = n; }

        private async Task<string> GetUserId()
            => (await _userManager.GetUserAsync(User))!.Id;

        public async Task<IActionResult> Index(
            string? status, string? company, DateTime? from, DateTime? to)
        {
            var id = await GetUserId();
            var apps = await _svc.GetStudentApplicationsAsync(id, status, company, from, to);
            ViewBag.Filter_Status  = status;
            ViewBag.Filter_Company = company;
            ViewBag.Filter_From    = from?.ToString("yyyy-MM-dd");
            ViewBag.Filter_To      = to?.ToString("yyyy-MM-dd");
            ViewBag.StatusList     = ApplicationFormViewModel.StatusList;
            return View(apps);
        }

        [HttpGet] public IActionResult Create() => RedirectToAction("Index", "JobBoard");
        [HttpPost, ValidateAntiForgeryToken] public IActionResult Create(ApplicationFormViewModel model) => RedirectToAction("Index", "JobBoard");

        [HttpGet] public async Task<IActionResult> Edit(int id)
        {
            var app = await _svc.GetByIdAsync(id);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();
            ViewBag.StatusList = ApplicationFormViewModel.StatusList;
            return View(new ApplicationFormViewModel {
                Id = app.Id, CompanyName = app.CompanyName, Role = app.Role,
                Status = app.Status, AppliedDate = app.AppliedDate,
                JobLink = app.JobLink, Notes = app.Notes,
                Location = app.Location, Package = app.Package
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ApplicationFormViewModel model)
        {
            if (!ModelState.IsValid)
            { ViewBag.StatusList = ApplicationFormViewModel.StatusList; return View(model); }

            var app = await _svc.GetByIdAsync(id);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();
            app.CompanyName = model.CompanyName; app.Role = model.Role;
            app.Status = model.Status; app.AppliedDate = model.AppliedDate;
            app.JobLink = model.JobLink; app.Notes = model.Notes;
            app.Location = model.Location; app.Package = model.Package;
            await _svc.UpdateAsync(app);
            TempData["Success"] = "Application updated!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var app = await _svc.GetByIdAsync(id);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();
            return View(app);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id, await GetUserId());
            TempData["Success"] = "Application removed.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var userId = await GetUserId();
            await _svc.UpdateStatusAsync(id, userId, status);
            var app = await _svc.GetByIdAsync(id);
            await _notificationSvc.SendAsync(userId, $"Update: Your application status for {app?.CompanyName} has been changed to {status}.");
            TempData["Success"] = $"Status updated to {status}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInterview(int appId,
            string interviewType, DateTime interviewDate,
            TimeSpan? interviewTime, string? meetingLink, string? venue, string? notes)
        {
            var app = await _svc.GetByIdAsync(appId);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();
            await _svc.AddInterviewAsync(new InterviewSchedule {
                JobApplicationId = appId, InterviewType = interviewType,
                InterviewDate = interviewDate, InterviewTime = interviewTime,
                MeetingLink = meetingLink, Venue = venue, Notes = notes, Outcome = "Pending"
            });
            TempData["Success"] = "Interview scheduled!";
            return RedirectToAction(nameof(Details), new { id = appId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInterview(int interviewId, int appId)
        {
            await _svc.DeleteInterviewAsync(interviewId);
            return RedirectToAction(nameof(Details), new { id = appId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInterviewOutcome(
            int interviewId, int appId, string outcome)
        {
            await _svc.UpdateInterviewOutcomeAsync(interviewId, outcome);
            return RedirectToAction(nameof(Details), new { id = appId });
        }

        public async Task<IActionResult> ExportExcel()
        {
            var id   = await GetUserId();
            var apps = await _svc.GetStudentApplicationsAsync(id);
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Placements");
            string[] headers = { "Company","Role","Status","Applied Date","Location","Package" };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i+1).Value = headers[i];
            int row = 2;
            foreach (var a in apps)
            {
                ws.Cell(row,1).Value = a.CompanyName;
                ws.Cell(row,2).Value = a.Role;
                ws.Cell(row,3).Value = a.Status;
                ws.Cell(row,4).Value = a.AppliedDate.ToString("dd-MM-yyyy");
                ws.Cell(row,5).Value = a.Location ?? "-";
                ws.Cell(row,6).Value = a.Package ?? "-";
                row++;
            }
            ws.Row(1).Style.Font.Bold = true;
            ws.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "MyPlacementApplications.xlsx");
        }
    }
}
