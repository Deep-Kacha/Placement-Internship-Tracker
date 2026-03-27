using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Models;

namespace PlacementTracker.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<JobDescription> JobDescriptions { get; set; }
        public DbSet<InterviewSchedule> InterviewSchedules { get; set; }
        public DbSet<InternshipApplication> InternshipApplications { get; set; }
        public DbSet<InternshipInterview> InternshipInterviews { get; set; }
        public DbSet<FacultyRemark> FacultyRemarks { get; set; }
        public DbSet<FacultyStudentMapping> FacultyStudentMappings { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<JobApplication>()
                .HasOne(a => a.Student)
                .WithMany(u => u.Applications)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobApplication>()
                .HasOne(a => a.JobDescription)
                .WithMany(j => j.JobApplications)
                .HasForeignKey(a => a.JobDescriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InternshipApplication>()
                .HasOne(a => a.Student)
                .WithMany(u => u.Internships)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InternshipApplication>()
                .HasOne(a => a.JobDescription)
                .WithMany(j => j.InternshipApplications)
                .HasForeignKey(a => a.JobDescriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InternshipInterview>()
                .HasOne(i => i.InternshipApplication)
                .WithMany(a => a.Interviews)
                .HasForeignKey(i => i.InternshipApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FacultyRemark>()
                .HasOne(r => r.Faculty)
                .WithMany(u => u.Remarks)
                .HasForeignKey(r => r.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FacultyRemark>()
                .HasOne(r => r.JobApplication)
                .WithMany(a => a.Remarks)
                .HasForeignKey(r => r.JobApplicationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FacultyRemark>()
                .HasOne(r => r.InternshipApplication)
                .WithMany(a => a.Remarks)
                .HasForeignKey(r => r.InternshipApplicationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FacultyStudentMapping>()
                .HasOne(m => m.Faculty)
                .WithMany()
                .HasForeignKey(m => m.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FacultyStudentMapping>()
                .HasOne(m => m.Student)
                .WithMany(u => u.FacultyMappings)
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FacultyStudentMapping>()
                .HasIndex(m => new { m.FacultyId, m.StudentId })
                .IsUnique();
        }
    }
}
