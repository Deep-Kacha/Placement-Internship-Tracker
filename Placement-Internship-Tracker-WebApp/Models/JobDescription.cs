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
        [DataType(DataType.Date)] public DateTime? CampusDriveDate { get; set; }

        public string? Location { get; set; }
        public string? PackageOrStipend { get; set; }
        public string? AnnualCTC { get; set; }
        public string? Bond { get; set; }
        public string? SelectionProcess { get; set; }
        public string? EligibleBatches { get; set; }
        public string? EligibleCourses { get; set; }
        public string? RegistrationLink { get; set; }
        public string? DocumentsPath { get; set; }

        // Recruiter (user who submitted)
        public string? RecruiterId { get; set; }
        public ApplicationUser? Recruiter { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation for students who applied
        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
        public ICollection<InternshipApplication> InternshipApplications { get; set; } = new List<InternshipApplication>();

        // Expanded Fields from TPPortal
        [StringLength(255)] public string? CompanyWebsite { get; set; }
        [StringLength(150)] public string? ContactName { get; set; }
        [StringLength(100)] public string? ContactEmail { get; set; }
        [StringLength(20)] public string? ContactMobile { get; set; }
        [StringLength(100)] public string? InternshipDuration { get; set; }
        [StringLength(100)] public string? Stipend { get; set; }
        [DataType(DataType.Date)] public DateTime? DateOfJoining { get; set; }
        public string? AdditionalDetails { get; set; }
    }
}
