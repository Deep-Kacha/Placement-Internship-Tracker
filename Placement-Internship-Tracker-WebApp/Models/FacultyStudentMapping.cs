namespace PlacementTracker.Models
{
    public class FacultyStudentMapping
    {
        public int Id { get; set; }
        public string FacultyId { get; set; } = string.Empty;
        public ApplicationUser? Faculty { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.Now;
    }
}
