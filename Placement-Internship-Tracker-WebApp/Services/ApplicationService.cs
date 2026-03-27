using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Services
{
    public class ApplicationService
    {
        private readonly AppDbContext _db;
        public ApplicationService(AppDbContext db) { _db = db; }

        public async Task<List<JobApplication>> GetStudentApplicationsAsync(
            string studentId, string? status = null, string? company = null,
            DateTime? from = null, DateTime? to = null)
        {
            var q = _db.JobApplications
                .Include(a => a.Interviews)
                .Include(a => a.Remarks).ThenInclude(r => r.Faculty)
                .Where(a => a.StudentId == studentId && a.IsActive);

            if (!string.IsNullOrEmpty(status)) q = q.Where(a => a.Status == status);
            if (!string.IsNullOrEmpty(company)) q = q.Where(a => a.CompanyName.Contains(company));
            if (from.HasValue) q = q.Where(a => a.AppliedDate >= from.Value);
            if (to.HasValue)   q = q.Where(a => a.AppliedDate <= to.Value);

            return await q.OrderByDescending(a => a.UpdatedAt).ToListAsync();
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
            => await _db.JobApplications
                .Include(a => a.Student)
                .Include(a => a.Interviews)
                .Include(a => a.Remarks).ThenInclude(r => r.Faculty)
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

        public async Task<JobApplication> CreateAsync(JobApplication app)
        {
            _db.JobApplications.Add(app);
            await _db.SaveChangesAsync();
            return app;
        }

        public async Task<bool> UpdateAsync(JobApplication app)
        {
            app.UpdatedAt = DateTime.Now;
            _db.JobApplications.Update(app);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id, string studentId)
        {
            var app = await _db.JobApplications.FirstOrDefaultAsync(a => a.Id == id && a.StudentId == studentId);
            if (app == null) return false;
            app.IsActive = false;
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStatusAsync(int id, string studentId, string newStatus)
        {
            var app = await _db.JobApplications.FirstOrDefaultAsync(a => a.Id == id && a.StudentId == studentId);
            if (app == null) return false;
            app.Status = newStatus;
            app.UpdatedAt = DateTime.Now;
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task AddInterviewAsync(InterviewSchedule interview)
        {
            _db.InterviewSchedules.Add(interview);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteInterviewAsync(int interviewId)
        {
            var iv = await _db.InterviewSchedules.FindAsync(interviewId);
            if (iv == null) return false;
            _db.InterviewSchedules.Remove(iv);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateInterviewOutcomeAsync(int interviewId, string outcome)
        {
            var iv = await _db.InterviewSchedules.FindAsync(interviewId);
            if (iv == null) return false;
            iv.Outcome = outcome;
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
