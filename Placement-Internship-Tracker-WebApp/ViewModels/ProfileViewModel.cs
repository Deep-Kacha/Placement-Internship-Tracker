using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Department { get; set; }

        [Display(Name = "College Roll Number")]
        public string? CollegeRollNo { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }
}
