using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.Models
{
    public class JobDescription
    {
        public int Id { get; set; }

        [Required, StringLength(200)] public string Title { get; set; } = string.Empty;
        
        [Required, StringLength(150)] public string CompanyName { get; set; } = string.Empty;
        
        [Required] public string Description { get; set; } = string.Empty;
        
        public string? Requirements { get; set; }

        [Required]
        public string JobType { get; set; } = "Placement"; // Placement or Internship

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        [DataType(DataType.Date)] public DateTime Deadline { get; set; }

        public string? Location { get; set; }
        public string? PackageOrStipend { get; set; }

        // Recruiter (user who submitted)
        public string? RecruiterId { get; set; }
        public ApplicationUser? Recruiter { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation for students who applied
        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
        public ICollection<InternshipApplication> InternshipApplications { get; set; } = new List<InternshipApplication>();
    }
}
