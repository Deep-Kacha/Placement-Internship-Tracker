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
    public class InternshipsController : Controller
    {
        private readonly InternshipService _svc;
        private readonly UserManager<ApplicationUser> _userManager;

        public InternshipsController(InternshipService s, UserManager<ApplicationUser> u)
        { _svc = s; _userManager = u; }

        private async Task<string> GetUserId()
            => (await _userManager.GetUserAsync(User))!.Id;

        public async Task<IActionResult> Index(
            string? status, string? company,
            string? internshipType, string? workMode,
            DateTime? from, DateTime? to)
        {
            var id = await GetUserId();
            var internships = await _svc.GetStudentInternshipsAsync(
                id, status, company, internshipType, workMode, from, to);

            ViewBag.Filter_Status        = status;
            ViewBag.Filter_Company       = company;
            ViewBag.Filter_InternshipType= internshipType;
            ViewBag.Filter_WorkMode      = workMode;
            ViewBag.Filter_From          = from?.ToString("yyyy-MM-dd");
            ViewBag.Filter_To            = to?.ToString("yyyy-MM-dd");
            ViewBag.StatusList           = InternshipFormViewModel.StatusList;
            ViewBag.TypeList             = InternshipFormViewModel.TypeList;
            ViewBag.ModeList             = InternshipFormViewModel.ModeList;
            return View(internships);
        }

        [HttpGet] public IActionResult Create() => RedirectToAction("Index", "JobBoard");
        [HttpPost, ValidateAntiForgeryToken] public IActionResult Create(InternshipFormViewModel model) => RedirectToAction("Index", "JobBoard");

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var app = await _svc.GetByIdAsync(id);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();
            ViewBag.StatusList = InternshipFormViewModel.StatusList;
            ViewBag.TypeList   = InternshipFormViewModel.TypeList;
            ViewBag.ModeList   = InternshipFormViewModel.ModeList;
            return View(new InternshipFormViewModel {
                Id = app.Id, CompanyName = app.CompanyName, Role = app.Role,
                InternshipType = app.InternshipType, WorkMode = app.WorkMode,
                Status = app.Status, AppliedDate = app.AppliedDate,
                StartDate = app.StartDate, EndDate = app.EndDate,
                Stipend = app.Stipend, Location = app.Location,
                JobLink = app.JobLink, Notes = app.Notes,
                IsPPOConverted = app.IsPPOConverted, PPOPackage = app.PPOPackage,
                CertificateReceived = app.CertificateReceived
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InternshipFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = InternshipFormViewModel.StatusList;
                ViewBag.TypeList   = InternshipFormViewModel.TypeList;
                ViewBag.ModeList   = InternshipFormViewModel.ModeList;
                return View(model);
            }
            var app = await _svc.GetByIdAsync(id);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();

            app.CompanyName = model.CompanyName; app.Role = model.Role;
            app.InternshipType = model.InternshipType; app.WorkMode = model.WorkMode;
            app.Status = model.Status; app.AppliedDate = model.AppliedDate;
            app.StartDate = model.StartDate; app.EndDate = model.EndDate;
            app.Stipend = model.Stipend; app.Location = model.Location;
            app.JobLink = model.JobLink; app.Notes = model.Notes;
            app.IsPPOConverted = model.IsPPOConverted; app.PPOPackage = model.PPOPackage;
            app.CertificateReceived = model.CertificateReceived;

            await _svc.UpdateAsync(app);
            TempData["Success"] = "Internship application updated!";
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
            TempData["Success"] = "Internship application removed.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _svc.UpdateStatusAsync(id, await GetUserId(), status);
            TempData["Success"] = $"Status updated to {status}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPPO(int id, string? ppoPackage)
        {
            await _svc.MarkPPOAsync(id, await GetUserId(), ppoPackage);
            TempData["Success"] = "Marked as PPO converted! 🎉";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkCertificate(int id)
        {
            await _svc.MarkCertificateAsync(id, await GetUserId());
            TempData["Success"] = "Certificate marked as received!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInterview(int appId,
            string roundType, DateTime interviewDate,
            TimeSpan? interviewTime, string? meetingLink, string? venue, string? notes)
        {
            var app = await _svc.GetByIdAsync(appId);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();
            await _svc.AddInterviewAsync(new InternshipInterview {
                InternshipApplicationId = appId, RoundType = roundType,
                InterviewDate = interviewDate, InterviewTime = interviewTime,
                MeetingLink = meetingLink, Venue = venue, Notes = notes, Outcome = "Pending"
            });
            TempData["Success"] = "Interview round scheduled!";
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
            var list = await _svc.GetStudentInternshipsAsync(id);
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Internships");
            string[] headers = {
                "Company","Role","Type","Mode","Status",
                "Applied Date","Start Date","End Date",
                "Stipend/Month","PPO","PPO Package","Certificate","Location","Notes"
            };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i+1).Value = headers[i];
            int row = 2;
            foreach (var a in list)
            {
                ws.Cell(row, 1).Value  = a.CompanyName;
                ws.Cell(row, 2).Value  = a.Role;
                ws.Cell(row, 3).Value  = a.InternshipType;
                ws.Cell(row, 4).Value  = a.WorkMode;
                ws.Cell(row, 5).Value  = a.Status;
                ws.Cell(row, 6).Value  = a.AppliedDate.ToString("dd-MM-yyyy");
                ws.Cell(row, 7).Value  = a.StartDate?.ToString("dd-MM-yyyy") ?? "-";
                ws.Cell(row, 8).Value  = a.EndDate?.ToString("dd-MM-yyyy") ?? "-";
                ws.Cell(row, 9).Value  = a.Stipend != null ? $"₹{a.Stipend}" : "-";
                ws.Cell(row, 10).Value = a.IsPPOConverted ? "Yes" : "No";
                ws.Cell(row, 11).Value = a.PPOPackage ?? "-";
                ws.Cell(row, 12).Value = a.CertificateReceived ? "Yes" : "No";
                ws.Cell(row, 13).Value = a.Location ?? "-";
                ws.Cell(row, 14).Value = a.Notes ?? "-";
                row++;
            }
            ws.Row(1).Style.Font.Bold = true;
            ws.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "MyInternships.xlsx");
        }
    }
}
