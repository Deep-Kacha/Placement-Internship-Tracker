using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.Models
{
    public class InternshipApplication
    {
        public int Id { get; set; }

        [Required] public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }

        [Required, StringLength(150)] public string CompanyName { get; set; } = string.Empty;
        [Required, StringLength(150)] public string Role { get; set; } = string.Empty;

        public int? JobDescriptionId { get; set; }
        public JobDescription? JobDescription { get; set; }

        [Required]
        public string Status { get; set; } = "Applied";

        [Required]
        public string InternshipType { get; set; } = "Summer";

        [Required]
        public string WorkMode { get; set; } = "On-Site";

        [DataType(DataType.Date)] public DateTime AppliedDate { get; set; } = DateTime.Today;
        [DataType(DataType.Date)] public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)] public DateTime? EndDate { get; set; }

        public string? Stipend { get; set; }
        public bool IsPPOConverted { get; set; } = false;
        public string? PPOPackage { get; set; }
        public bool CertificateReceived { get; set; } = false;

        public string? Location { get; set; }
        [Url] public string? JobLink { get; set; }
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<InternshipInterview> Interviews { get; set; } = new List<InternshipInterview>();
        public ICollection<FacultyRemark> Remarks { get; set; } = new List<FacultyRemark>();

        // Computed helpers
        public int? DurationDays => (StartDate.HasValue && EndDate.HasValue)
            ? (int)(EndDate.Value - StartDate.Value).TotalDays : null;
        public bool IsOngoing => Status == "Selected" && StartDate.HasValue
            && StartDate <= DateTime.Today && (!EndDate.HasValue || EndDate >= DateTime.Today);
    }
}
