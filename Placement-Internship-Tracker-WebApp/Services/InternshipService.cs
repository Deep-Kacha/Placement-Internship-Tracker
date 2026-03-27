using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;

namespace PlacementTracker.Services
{
    public class InternshipService
    {
        private readonly AppDbContext _db;
        public InternshipService(AppDbContext db) { _db = db; }

        public async Task<List<InternshipApplication>> GetStudentInternshipsAsync(
            string studentId, string? status = null, string? company = null,
            string? internshipType = null, string? workMode = null,
            DateTime? from = null, DateTime? to = null)
        {
            var q = _db.InternshipApplications
                .Include(a => a.Interviews)
                .Include(a => a.Remarks).ThenInclude(r => r.Faculty)
                .Where(a => a.StudentId == studentId && a.IsActive);

            if (!string.IsNullOrEmpty(status))        q = q.Where(a => a.Status == status);
            if (!string.IsNullOrEmpty(company))       q = q.Where(a => a.CompanyName.Contains(company));
            if (!string.IsNullOrEmpty(internshipType))q = q.Where(a => a.InternshipType == internshipType);
            if (!string.IsNullOrEmpty(workMode))      q = q.Where(a => a.WorkMode == workMode);
            if (from.HasValue) q = q.Where(a => a.AppliedDate >= from.Value);
            if (to.HasValue)   q = q.Where(a => a.AppliedDate <= to.Value);

            return await q.OrderByDescending(a => a.UpdatedAt).ToListAsync();
        }

        public async Task<InternshipApplication?> GetByIdAsync(int id)
            => await _db.InternshipApplications
                .Include(a => a.Student)
                .Include(a => a.Interviews)
                .Include(a => a.Remarks).ThenInclude(r => r.Faculty)
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

        public async Task<InternshipApplication> CreateAsync(InternshipApplication app)
        {
            _db.InternshipApplications.Add(app);
            await _db.SaveChangesAsync();
            return app;
        }

        public async Task<bool> UpdateAsync(InternshipApplication app)
        {
            app.UpdatedAt = DateTime.Now;
            _db.InternshipApplications.Update(app);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id, string studentId)
        {
            var app = await _db.InternshipApplications
                .FirstOrDefaultAsync(a => a.Id == id && a.StudentId == studentId);
            if (app == null) return false;
            app.IsActive = false;
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStatusAsync(int id, string studentId, string newStatus)
        {
            var app = await _db.InternshipApplications
                .FirstOrDefaultAsync(a => a.Id == id && a.StudentId == studentId);
            if (app == null) return false;
            app.Status = newStatus;
            app.UpdatedAt = DateTime.Now;
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> MarkFullTimeOfferedAsync(int id, string studentId, string? fullTimePackage)
        {
            var app = await _db.InternshipApplications
                .FirstOrDefaultAsync(a => a.Id == id && a.StudentId == studentId);
            if (app == null) return false;
            app.IsFullTimeOffered = true;
            app.FullTimePackage = fullTimePackage;
            app.UpdatedAt = DateTime.Now;
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> MarkCertificateAsync(int id, string studentId)
        {
            var app = await _db.InternshipApplications
                .FirstOrDefaultAsync(a => a.Id == id && a.StudentId == studentId);
            if (app == null) return false;
            app.CertificateReceived = true;
            app.UpdatedAt = DateTime.Now;
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task AddInterviewAsync(InternshipInterview interview)
        {
            _db.InternshipInterviews.Add(interview);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteInterviewAsync(int interviewId)
        {
            var iv = await _db.InternshipInterviews.FindAsync(interviewId);
            if (iv == null) return false;
            _db.InternshipInterviews.Remove(iv);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateInterviewOutcomeAsync(int interviewId, string outcome)
        {
            var iv = await _db.InternshipInterviews.FindAsync(interviewId);
            if (iv == null) return false;
            iv.Outcome = outcome;
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<List<InternshipApplication>> GetAllInternshipsAsync(
            string? status = null, string? department = null,
            string? company = null, string? internshipType = null)
        {
            var q = _db.InternshipApplications
                .Include(a => a.Student)
                .Where(a => a.IsActive).AsQueryable();

            if (!string.IsNullOrEmpty(status))        q = q.Where(a => a.Status == status);
            if (!string.IsNullOrEmpty(company))       q = q.Where(a => a.CompanyName.Contains(company));
            if (!string.IsNullOrEmpty(internshipType))q = q.Where(a => a.InternshipType == internshipType);
            if (!string.IsNullOrEmpty(department))    q = q.Where(a => a.Student!.Department == department);

            return await q.OrderByDescending(a => a.UpdatedAt).ToListAsync();
        }
    }
}
