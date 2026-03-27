using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.Models
{
    public class JobApplication
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
        // Status flow: Applied → Screening → Interview → Offer | Rejected | Withdrawn

        [DataType(DataType.Date)] public DateTime AppliedDate { get; set; } = DateTime.Today;
        [Url] public string? JobLink { get; set; }
        public string? Notes { get; set; }
        public string? Location { get; set; }
        public string? Package { get; set; }  // e.g. "6 LPA"
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<InterviewSchedule> Interviews { get; set; } = new List<InterviewSchedule>();
        public ICollection<FacultyRemark> Remarks { get; set; } = new List<FacultyRemark>();
    }
}
