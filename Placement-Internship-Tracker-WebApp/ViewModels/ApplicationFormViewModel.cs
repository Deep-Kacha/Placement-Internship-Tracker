using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.ViewModels
{
    public class ApplicationFormViewModel
    {
        public int Id { get; set; }
        [Required] public string CompanyName { get; set; } = string.Empty;
        [Required] public string Role { get; set; } = string.Empty;
        [Required] public string Status { get; set; } = "Applied";
        [DataType(DataType.Date)] public DateTime AppliedDate { get; set; } = DateTime.Today;
        [Url] public string? JobLink { get; set; }
        public string? Notes { get; set; }
        public string? Location { get; set; }
        public string? Package { get; set; }

        public static List<string> StatusList => new() {
            "Applied", "Screening", "Interview", "Offer", "Selected", "Rejected", "Withdrawn"
        };
    }
}
