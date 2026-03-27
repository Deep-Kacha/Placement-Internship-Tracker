using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.ViewModels
{
    public class InternshipFormViewModel
    {
        public int Id { get; set; }

        [Required, Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty;

        [Required, Display(Name = "Role / Designation")]
        public string Role { get; set; } = string.Empty;

        [Required, Display(Name = "Internship Type")]
        public string InternshipType { get; set; } = "Summer";

        [Required, Display(Name = "Work Mode")]
        public string WorkMode { get; set; } = "On-Site";

        [Required, Display(Name = "Application Status")]
        public string Status { get; set; } = "Applied";

        [DataType(DataType.Date), Display(Name = "Applied Date")]
        public DateTime AppliedDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date), Display(Name = "Internship Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date), Display(Name = "Internship End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Monthly Stipend (₹)")]
        public string? Stipend { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Url, Display(Name = "Job / Portal Link")]
        public string? JobLink { get; set; }

        [Display(Name = "Notes / Remarks")]
        public string? Notes { get; set; }

        [Display(Name = "Full-time Offered")]
        public bool IsFullTimeOffered { get; set; } = false;

        [Display(Name = "Full-time Package (if offered)")]
        public string? FullTimePackage { get; set; }

        [Display(Name = "Certificate Received")]
        public bool CertificateReceived { get; set; } = false;

        public static List<string> StatusList => new()
        {
            "Applied", "Shortlisted", "Test", "Interview",
            "Selected", "Rejected", "Withdrawn", "Completed"
        };

        public static List<string> TypeList => new()
        {
            "Summer", "Winter", "Industrial Training", "Research", "Full-time Offered", "Part-Time"
        };

        public static List<string> ModeList => new()
        {
            "On-Site", "Remote", "Hybrid"
        };
    }
}
