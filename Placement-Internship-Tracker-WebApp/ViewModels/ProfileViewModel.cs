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

        public string? Branch { get; set; }
        public string? Semester { get; set; }
        public double? CGPA { get; set; }

        [Display(Name = "10th Percentage")]
        public double? TenthPercentage { get; set; }
        [Display(Name = "12th Percentage")]
        public double? TwelfthPercentage { get; set; }
        [Display(Name = "Diploma Percentage")]
        public double? DiplomaPercentage { get; set; }

        [Display(Name = "Parent Name")]
        public string? ParentName { get; set; }
        [Display(Name = "Parent Contact")]
        public string? ParentContact { get; set; }
        [Display(Name = "Parent Email")]
        public string? ParentEmail { get; set; }

        [Display(Name = "Current Address")]
        public string? CurrentAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }
}
