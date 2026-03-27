using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.ViewModels
{
    public class RegisterViewModel
    {
        [Required] public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, DataType(DataType.Password), MinLength(6)] public string Password { get; set; } = string.Empty;
        [Compare("Password")] public string ConfirmPassword { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? CollegeRollNo { get; set; }
        public string? Branch { get; set; }
        public string? Semester { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; } = "Student";
    }
}
