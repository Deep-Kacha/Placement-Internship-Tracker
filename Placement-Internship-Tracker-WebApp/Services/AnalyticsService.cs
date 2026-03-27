using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Services
{
    public class AnalyticsService
    {
        private readonly AppDbContext _db;
        public AnalyticsService(AppDbContext db) { _db = db; }

        public async Task<DashboardViewModel> GetStudentDashboardAsync(string studentId)
        {
            var apps = await _db.JobApplications
                .Include(a => a.Interviews)
                .Where(a => a.StudentId == studentId && a.IsActive)
                .ToListAsync();

            var internships = await _db.InternshipApplications
                .Include(a => a.Interviews)
                .Where(a => a.StudentId == studentId && a.IsActive)
                .ToListAsync();

            var upcomingPlacement = apps.SelectMany(a => a.Interviews)
                .Where(i => i.InterviewDate >= DateTime.Today)
                .OrderBy(i => i.InterviewDate).Take(5).ToList();

            var upcomingInternship = internships.SelectMany(a => a.Interviews)
                .Where(i => i.InterviewDate >= DateTime.Today)
                .OrderBy(i => i.InterviewDate).Take(5).ToList();

            var notifs = await _db.Notifications
                .Where(n => n.UserId == studentId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt).Take(10).ToListAsync();

            return new DashboardViewModel
            {
                TotalApplications = apps.Count,
                Applied    = apps.Count(a => a.Status == "Applied"),
                Screening  = apps.Count(a => a.Status == "Screening"),
                Interview  = apps.Count(a => a.Status == "Interview"),
                Offer      = apps.Count(a => a.Status == "Offer"),
                Rejected   = apps.Count(a => a.Status == "Rejected"),
                Withdrawn  = apps.Count(a => a.Status == "Withdrawn"),

                TotalInternships        = internships.Count,
                InternshipApplied       = internships.Count(a => a.Status == "Applied"),
                InternshipShortlisted   = internships.Count(a => a.Status == "Shortlisted"),
                InternshipSelected      = internships.Count(a => a.Status == "Selected"),
                InternshipCompleted     = internships.Count(a => a.Status == "Completed"),
                InternshipRejected      = internships.Count(a => a.Status == "Rejected"),
                FullTimeOffered          = internships.Count(a => a.IsFullTimeOffered),
                HasOngoingInternship    = internships.Any(a => a.IsOngoing),

                UpcomingInterviews           = upcomingPlacement,
                UpcomingInternshipInterviews = upcomingInternship,
                RecentApplications = apps.OrderByDescending(a => a.UpdatedAt).Take(5).ToList(),
                RecentInternships  = internships.OrderByDescending(a => a.UpdatedAt).Take(5).ToList(),
                Notifications = notifs
            };
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardAsync()
        {
            var apps        = await _db.JobApplications.Include(a=>a.Student).Where(a => a.IsActive).ToListAsync();
            var internships = await _db.InternshipApplications.Include(a=>a.Student).Where(a => a.IsActive).ToListAsync();
            var users       = await _db.Users.ToListAsync();

            var roleMap = (await _db.UserRoles.ToListAsync())
                .Join(await _db.Roles.ToListAsync(),
                    ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name });
            var studentIds = roleMap.Where(x => x.Name == "Student").Select(x => x.UserId).ToHashSet();
            var facultyIds = roleMap.Where(x => x.Name == "Faculty").Select(x => x.UserId).ToHashSet();

            var offers     = apps.Where(a => a.Status == "Offer").ToList();
            int totalStudents   = studentIds.Count;

            return new AdminDashboardViewModel
            {
                TotalStudents     = totalStudents,
                TotalFaculty      = facultyIds.Count,
                TotalApplications = apps.Count,
                TotalOffers       = offers.Count,
                TotalInterviews   = apps.Count(a => a.Status == "Interview"),
                PlacementRate     = totalStudents > 0
                    ? Math.Round((double)offers.Count / totalStudents * 100, 1) : 0,

                TotalInternships         = internships.Count,
                TotalInternshipSelected  = internships.Count(a => a.Status == "Selected"),
                TotalInternshipCompleted = internships.Count(a => a.Status == "Completed"),
                TotalFullTimeOffered       = internships.Count(a => a.IsFullTimeOffered),
                InternshipConversionRate = internships.Count > 0
                    ? Math.Round((double)internships.Count(a => a.IsFullTimeOffered) / internships.Count * 100, 1) : 0,

                StatusDistribution = new Dictionary<string, int>
                {
                    ["Applied"]   = apps.Count(a => a.Status == "Applied"),
                    ["Screening"] = apps.Count(a => a.Status == "Screening"),
                    ["Interview"] = apps.Count(a => a.Status == "Interview"),
                    ["Offer"]     = apps.Count(a => a.Status == "Offer"),
                    ["Rejected"]  = apps.Count(a => a.Status == "Rejected"),
                },

                InternshipStatusDistribution = new Dictionary<string, int>
                {
                    ["Applied"]     = internships.Count(a => a.Status == "Applied"),
                    ["Shortlisted"] = internships.Count(a => a.Status == "Shortlisted"),
                    ["Selected"]    = internships.Count(a => a.Status == "Selected"),
                    ["Completed"]   = internships.Count(a => a.Status == "Completed"),
                    ["Rejected"]    = internships.Count(a => a.Status == "Rejected"),
                },

                InternshipTypeDistribution = internships
                    .GroupBy(a => a.InternshipType)
                    .ToDictionary(g => g.Key, g => g.Count()),

                TopCompanies = apps.GroupBy(a => a.CompanyName)
                    .OrderByDescending(g => g.Count()).Take(5)
                    .ToDictionary(g => g.Key, g => g.Count()),

                TopInternshipCompanies = internships.GroupBy(a => a.CompanyName)
                    .OrderByDescending(g => g.Count()).Take(5)
                    .ToDictionary(g => g.Key, g => g.Count()),

                RecentActivity = apps.OrderByDescending(a => a.UpdatedAt).Take(10)
                    .Select(a => new RecentActivityItem {
                        StudentName = a.Student?.FullName ?? "—",
                        CompanyName = a.CompanyName,
                        Status      = a.Status,
                        UpdatedAt   = a.UpdatedAt
                    }).ToList(),

                RecentInternshipActivity = internships.OrderByDescending(a => a.UpdatedAt).Take(10)
                    .Select(a => new RecentInternshipActivityItem {
                        StudentName    = a.Student?.FullName ?? "—",
                        CompanyName    = a.CompanyName,
                        InternshipType = a.InternshipType,
                        Status         = a.Status,
                        UpdatedAt      = a.UpdatedAt
                    }).ToList()
            };
        }
    }
}
