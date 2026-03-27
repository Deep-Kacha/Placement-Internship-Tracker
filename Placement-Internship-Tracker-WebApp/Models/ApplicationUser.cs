using Microsoft.AspNetCore.Identity;

namespace PlacementTracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? CollegeRollNo { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public bool IsApproved { get; set; } = false; // Requires Admin approval

        // Navigation
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
        public ICollection<InternshipApplication> Internships { get; set; } = new List<InternshipApplication>();
        public ICollection<FacultyRemark> Remarks { get; set; } = new List<FacultyRemark>();
        public ICollection<FacultyStudentMapping> FacultyMappings { get; set; } = new List<FacultyStudentMapping>();
    }
}
