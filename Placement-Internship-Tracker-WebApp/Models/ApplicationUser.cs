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

        // Academic Details
        public double? TenthPercentage { get; set; }
        public double? TwelfthPercentage { get; set; }
        public double? CGPA { get; set; }
        public double? DiplomaPercentage { get; set; }
        public string? Branch { get; set; }
        public string? Semester { get; set; }

        // Parent Information
        public string? ParentName { get; set; }
        public string? ParentEmail { get; set; }
        public string? ParentMobile { get; set; }

        // Address
        public string? CurrentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }

        // Documents
        public string? ResumePath { get; set; }

        // Navigation
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
        public ICollection<InternshipApplication> Internships { get; set; } = new List<InternshipApplication>();
        public ICollection<FacultyRemark> Remarks { get; set; } = new List<FacultyRemark>();
        public ICollection<FacultyStudentMapping> FacultyMappings { get; set; } = new List<FacultyStudentMapping>();
    }
}
