# ╔══════════════════════════════════════════════════════════════════╗
# ║   PLACEMENT & INTERNSHIP TRACKER — FULL IMPLEMENTATION v2       ║
# ║   ASP.NET MVC 8.0 | EF Core | Identity | Bootstrap 5            ║
# ║   ✦ NOW INCLUDES DEDICATED INTERNSHIP TRACKING MODULE ✦         ║
# ╚══════════════════════════════════════════════════════════════════╝

You are a senior .NET developer. Build me a complete, production-ready
**Placement & Internship Tracker** web application using ASP.NET MVC 8.0.
The app tracks TWO separate pipelines:
  1. **Job/Placement Applications** — full-time roles after graduation
  2. **Internship Applications** — summer/winter/industrial internships with
     stipend, duration, mode, and PPO-conversion tracking

Follow every instruction below EXACTLY. Generate complete, runnable code
for every file listed. Do NOT skip any section.

---

## ═══════════════════════════════
## SECTION 1 — PROJECT SETUP
## ═══════════════════════════════

### 1.1 Create Project
```bash
dotnet new mvc -n PlacementTracker --framework net8.0
cd PlacementTracker
```

### 1.2 Install NuGet Packages
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.AspNetCore.Identity.UI --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package ClosedXML --version 0.102.2
```

### 1.3 `appsettings.json` — Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PlacementTrackerDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" }
  },
  "AllowedHosts": "*"
}
```

---

## ═══════════════════════════════
## SECTION 2 — MODELS
## ═══════════════════════════════

### FILE: `Models/ApplicationUser.cs`
```csharp
using Microsoft.AspNetCore.Identity;

namespace PlacementTracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? CollegeRollNo { get; set; }
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
        public ICollection<InternshipApplication> Internships { get; set; } = new List<InternshipApplication>();
        public ICollection<FacultyRemark> Remarks { get; set; } = new List<FacultyRemark>();
        public ICollection<FacultyStudentMapping> FacultyMappings { get; set; } = new List<FacultyStudentMapping>();
    }
}
```

### FILE: `Models/JobApplication.cs`
```csharp
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
```

### FILE: `Models/InternshipApplication.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.Models
{
    /// <summary>
    /// Dedicated model for internship applications — separate from full-time placements.
    /// Tracks stipend, duration, work mode, type, and PPO conversion.
    /// </summary>
    public class InternshipApplication
    {
        public int Id { get; set; }

        [Required] public string StudentId { get; set; } = string.Empty;
        public ApplicationUser? Student { get; set; }

        [Required, StringLength(150)] public string CompanyName { get; set; } = string.Empty;
        [Required, StringLength(150)] public string Role { get; set; } = string.Empty;

        // ── Status Flow ──
        // Applied → Shortlisted → Test → Interview → Selected | Rejected | Withdrawn | Completed
        [Required]
        public string Status { get; set; } = "Applied";

        // ── Internship Type ──
        // Summer | Winter | Industrial Training | Research | PPO | Part-Time
        [Required]
        public string InternshipType { get; set; } = "Summer";

        // ── Work Mode ──
        // Remote | On-Site | Hybrid
        [Required]
        public string WorkMode { get; set; } = "On-Site";

        [DataType(DataType.Date)] public DateTime AppliedDate { get; set; } = DateTime.Today;
        [DataType(DataType.Date)] public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)] public DateTime? EndDate { get; set; }

        // Stipend amount per month, e.g. "15000", "25000"
        public string? Stipend { get; set; }

        // Whether the internship resulted in a Pre-Placement Offer
        public bool IsPPOConverted { get; set; } = false;
        public string? PPOPackage { get; set; }  // e.g. "8 LPA" (filled if PPO = true)

        // Certificate received after completion
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

        // ── Computed helpers ──
        public int? DurationDays => (StartDate.HasValue && EndDate.HasValue)
            ? (int)(EndDate.Value - StartDate.Value).TotalDays : null;
        public bool IsOngoing => Status == "Selected" && StartDate.HasValue
            && StartDate <= DateTime.Today && (!EndDate.HasValue || EndDate >= DateTime.Today);
    }
}
```

### FILE: `Models/InternshipInterview.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.Models
{
    /// <summary>
    /// Interview rounds specific to an internship application.
    /// </summary>
    public class InternshipInterview
    {
        public int Id { get; set; }
        public int InternshipApplicationId { get; set; }
        public InternshipApplication? InternshipApplication { get; set; }

        [Required]
        public string RoundType { get; set; } = "Technical";
        // Technical | HR | Aptitude | Group Discussion | Assignment | Final

        [Required, DataType(DataType.Date)] public DateTime InterviewDate { get; set; }
        [DataType(DataType.Time)] public TimeSpan? InterviewTime { get; set; }
        [Url] public string? MeetingLink { get; set; }
        public string? Venue { get; set; }
        public string? Notes { get; set; }
        public string? Outcome { get; set; } // Cleared | Not Cleared | Pending
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
```

### FILE: `Models/InterviewSchedule.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.Models
{
    public class InterviewSchedule
    {
        public int Id { get; set; }
        public int JobApplicationId { get; set; }
        public JobApplication? JobApplication { get; set; }

        [Required] public string InterviewType { get; set; } = "Technical"; // Technical | HR | Group Discussion | Aptitude | Final
        [Required, DataType(DataType.Date)] public DateTime InterviewDate { get; set; }
        [DataType(DataType.Time)]  public TimeSpan? InterviewTime { get; set; }
        [Url] public string? MeetingLink { get; set; }
        public string? Venue { get; set; }
        public string? Notes { get; set; }
        public string? Outcome { get; set; }  // Cleared | Not Cleared | Pending
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
```

### FILE: `Models/FacultyRemark.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.Models
{
    public class FacultyRemark
    {
        public int Id { get; set; }
        [Required] public string FacultyId { get; set; } = string.Empty;
        public ApplicationUser? Faculty { get; set; }

        // Remark can be on either a JobApplication OR an InternshipApplication
        public int? JobApplicationId { get; set; }
        public JobApplication? JobApplication { get; set; }

        public int? InternshipApplicationId { get; set; }
        public InternshipApplication? InternshipApplication { get; set; }

        [Required, StringLength(1000)] public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
```

### FILE: `Models/FacultyStudentMapping.cs`
```csharp
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
```

### FILE: `Models/Company.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required, StringLength(150)] public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Industry { get; set; }
        public string? Website { get; set; }
        public bool OffersInternships { get; set; } = true;
        public bool OffersPlacement { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
```

### FILE: `Models/Notification.cs`
```csharp
namespace PlacementTracker.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Link { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
```

---

## ═══════════════════════════════
## SECTION 3 — VIEWMODELS
## ═══════════════════════════════

### FILE: `ViewModels/LoginViewModel.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.ViewModels
{
    public class LoginViewModel
    {
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
```

### FILE: `ViewModels/RegisterViewModel.cs`
```csharp
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
        public string? Phone { get; set; }
        public string Role { get; set; } = "Student";
    }
}
```

### FILE: `ViewModels/ApplicationFormViewModel.cs`
```csharp
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
            "Applied", "Screening", "Interview", "Offer", "Rejected", "Withdrawn"
        };
    }
}
```

### FILE: `ViewModels/InternshipFormViewModel.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace PlacementTracker.ViewModels
{
    /// <summary>
    /// ViewModel for creating and editing internship applications.
    /// Includes fields unique to internships: type, duration, stipend, mode, PPO.
    /// </summary>
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

        [Display(Name = "PPO Converted")]
        public bool IsPPOConverted { get; set; } = false;

        [Display(Name = "PPO Package (if converted)")]
        public string? PPOPackage { get; set; }

        [Display(Name = "Certificate Received")]
        public bool CertificateReceived { get; set; } = false;

        // ── Static reference lists ──
        public static List<string> StatusList => new()
        {
            "Applied", "Shortlisted", "Test", "Interview",
            "Selected", "Rejected", "Withdrawn", "Completed"
        };

        public static List<string> TypeList => new()
        {
            "Summer", "Winter", "Industrial Training", "Research", "PPO", "Part-Time"
        };

        public static List<string> ModeList => new()
        {
            "On-Site", "Remote", "Hybrid"
        };
    }
}
```

### FILE: `ViewModels/DashboardViewModel.cs`
```csharp
using PlacementTracker.Models;

namespace PlacementTracker.ViewModels
{
    public class DashboardViewModel
    {
        // ── Job/Placement Summary ──
        public int TotalApplications { get; set; }
        public int Applied { get; set; }
        public int Screening { get; set; }
        public int Interview { get; set; }
        public int Offer { get; set; }
        public int Rejected { get; set; }
        public int Withdrawn { get; set; }

        // ── Internship Summary ──
        public int TotalInternships { get; set; }
        public int InternshipApplied { get; set; }
        public int InternshipShortlisted { get; set; }
        public int InternshipSelected { get; set; }
        public int InternshipCompleted { get; set; }
        public int InternshipRejected { get; set; }
        public int PPOConverted { get; set; }
        public bool HasOngoingInternship { get; set; }

        // ── Upcoming & Recent ──
        public List<InterviewSchedule> UpcomingInterviews { get; set; } = new();
        public List<InternshipInterview> UpcomingInternshipInterviews { get; set; } = new();
        public List<JobApplication> RecentApplications { get; set; } = new();
        public List<InternshipApplication> RecentInternships { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();
    }
}
```

### FILE: `ViewModels/AdminDashboardViewModel.cs`
```csharp
namespace PlacementTracker.ViewModels
{
    public class AdminDashboardViewModel
    {
        // ── People ──
        public int TotalStudents { get; set; }
        public int TotalFaculty { get; set; }

        // ── Placements ──
        public int TotalApplications { get; set; }
        public int TotalOffers { get; set; }
        public int TotalInterviews { get; set; }
        public double PlacementRate { get; set; }

        // ── Internships ──
        public int TotalInternships { get; set; }
        public int TotalInternshipSelected { get; set; }
        public int TotalInternshipCompleted { get; set; }
        public int TotalPPOConverted { get; set; }
        public double InternshipConversionRate { get; set; }

        // ── Charts ──
        public Dictionary<string, int> DepartmentWiseOffers { get; set; } = new();
        public Dictionary<string, int> StatusDistribution { get; set; } = new();
        public Dictionary<string, int> InternshipStatusDistribution { get; set; } = new();
        public Dictionary<string, int> TopCompanies { get; set; } = new();
        public Dictionary<string, int> TopInternshipCompanies { get; set; } = new();
        public Dictionary<string, int> InternshipTypeDistribution { get; set; } = new();

        public List<RecentActivityItem> RecentActivity { get; set; } = new();
        public List<RecentInternshipActivityItem> RecentInternshipActivity { get; set; } = new();
    }

    public class RecentActivityItem
    {
        public string StudentName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }

    public class RecentInternshipActivityItem
    {
        public string StudentName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string InternshipType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }
}
```

### FILE: `ViewModels/FacultyDashboardViewModel.cs`
```csharp
using PlacementTracker.Models;

namespace PlacementTracker.ViewModels
{
    public class FacultyDashboardViewModel
    {
        public List<StudentProgressItem> Students { get; set; } = new();
        public List<InterviewSchedule> UpcomingInterviews { get; set; } = new();
        public List<InternshipInterview> UpcomingInternshipInterviews { get; set; } = new();
        public int TotalStudents { get; set; }
        public int TotalApplications { get; set; }
        public int TotalOffers { get; set; }
        public int TotalInternships { get; set; }
        public int TotalInternshipSelected { get; set; }
        public int TotalPPOConverted { get; set; }
    }

    public class StudentProgressItem
    {
        public ApplicationUser Student { get; set; } = null!;
        public int TotalApps { get; set; }
        public int Offers { get; set; }
        public int Interviews { get; set; }
        public string LatestStatus { get; set; } = string.Empty;
        public int TotalInternships { get; set; }
        public int InternshipSelected { get; set; }
        public bool HasPPO { get; set; }
        public bool HasOngoingInternship { get; set; }
    }
}
```

---

## ═══════════════════════════════
## SECTION 4 — DATA / DB CONTEXT
## ═══════════════════════════════

### FILE: `Data/AppDbContext.cs`
```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Models;

namespace PlacementTracker.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ── Placement ──
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<InterviewSchedule> InterviewSchedules { get; set; }

        // ── Internship ──
        public DbSet<InternshipApplication> InternshipApplications { get; set; }
        public DbSet<InternshipInterview> InternshipInterviews { get; set; }

        // ── Shared ──
        public DbSet<FacultyRemark> FacultyRemarks { get; set; }
        public DbSet<FacultyStudentMapping> FacultyStudentMappings { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // JobApplication → Student (restrict delete)
            builder.Entity<JobApplication>()
                .HasOne(a => a.Student)
                .WithMany(u => u.Applications)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // InternshipApplication → Student (restrict delete)
            builder.Entity<InternshipApplication>()
                .HasOne(a => a.Student)
                .WithMany(u => u.Internships)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // InternshipInterview → InternshipApplication (cascade)
            builder.Entity<InternshipInterview>()
                .HasOne(i => i.InternshipApplication)
                .WithMany(a => a.Interviews)
                .HasForeignKey(i => i.InternshipApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // FacultyRemark → Faculty (restrict)
            builder.Entity<FacultyRemark>()
                .HasOne(r => r.Faculty)
                .WithMany(u => u.Remarks)
                .HasForeignKey(r => r.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            // FacultyRemark → JobApplication (nullable, cascade)
            builder.Entity<FacultyRemark>()
                .HasOne(r => r.JobApplication)
                .WithMany(a => a.Remarks)
                .HasForeignKey(r => r.JobApplicationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // FacultyRemark → InternshipApplication (nullable, cascade)
            builder.Entity<FacultyRemark>()
                .HasOne(r => r.InternshipApplication)
                .WithMany(a => a.Remarks)
                .HasForeignKey(r => r.InternshipApplicationId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // Faculty–Student unique mapping
            builder.Entity<FacultyStudentMapping>()
                .HasIndex(m => new { m.FacultyId, m.StudentId })
                .IsUnique();
        }
    }
}
```

### FILE: `Data/SeedData.cs`
```csharp
using Microsoft.AspNetCore.Identity;
using PlacementTracker.Models;

namespace PlacementTracker.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var context = services.GetRequiredService<AppDbContext>();

            await context.Database.EnsureCreatedAsync();

            // ── Seed Roles ──
            string[] roles = { "SuperAdmin", "Admin", "Faculty", "Student" };
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // ── SuperAdmin ──
            await CreateUser(userManager, new ApplicationUser {
                UserName = "superadmin@placement.com",
                Email = "superadmin@placement.com",
                FullName = "Super Admin",
                Department = "Management",
                EmailConfirmed = true
            }, "Admin@123", "SuperAdmin");

            // ── Admin ──
            var admin = await CreateUser(userManager, new ApplicationUser {
                UserName = "admin@placement.com",
                Email = "admin@placement.com",
                FullName = "Placement Admin",
                Department = "Placement Cell",
                EmailConfirmed = true
            }, "Admin@123", "Admin");

            // ── Faculty ──
            var faculty = await CreateUser(userManager, new ApplicationUser {
                UserName = "faculty@placement.com",
                Email = "faculty@placement.com",
                FullName = "Prof. Anjali Mehta",
                Department = "Computer Science",
                EmailConfirmed = true
            }, "Faculty@123", "Faculty");

            // ── Students ──
            var student1 = await CreateUser(userManager, new ApplicationUser {
                UserName = "rahul@student.com",
                Email = "rahul@student.com",
                FullName = "Rahul Sharma",
                Department = "Computer Science",
                CollegeRollNo = "CS2021001",
                EmailConfirmed = true
            }, "Student@123", "Student");

            var student2 = await CreateUser(userManager, new ApplicationUser {
                UserName = "priya@student.com",
                Email = "priya@student.com",
                FullName = "Priya Patel",
                Department = "Information Technology",
                CollegeRollNo = "IT2021015",
                EmailConfirmed = true
            }, "Student@123", "Student");

            // ── Seed Companies ──
            if (!context.Companies.Any())
            {
                context.Companies.AddRange(
                    new Company { Name = "Google",     Location = "Bangalore",  Industry = "Technology",      OffersInternships = true,  OffersPlacement = true },
                    new Company { Name = "Microsoft",  Location = "Hyderabad",  Industry = "Technology",      OffersInternships = true,  OffersPlacement = true },
                    new Company { Name = "Infosys",    Location = "Pune",       Industry = "IT Services",     OffersInternships = true,  OffersPlacement = true },
                    new Company { Name = "TCS",        Location = "Mumbai",     Industry = "IT Services",     OffersInternships = true,  OffersPlacement = true },
                    new Company { Name = "Amazon",     Location = "Bangalore",  Industry = "E-Commerce/Cloud",OffersInternships = true,  OffersPlacement = true },
                    new Company { Name = "Flipkart",   Location = "Bangalore",  Industry = "E-Commerce",      OffersInternships = true,  OffersPlacement = false },
                    new Company { Name = "DRDO",       Location = "Delhi",      Industry = "Research/Defence", OffersInternships = true,  OffersPlacement = false }
                );
                await context.SaveChangesAsync();
            }

            // ── Seed Placement Applications for Student 1 ──
            if (student1 != null && !context.JobApplications.Any(a => a.StudentId == student1.Id))
            {
                var app1 = new JobApplication {
                    StudentId = student1.Id,
                    CompanyName = "Google", Role = "SDE Intern",
                    Status = "Offer", AppliedDate = DateTime.Now.AddDays(-30),
                    Package = "80,000/month", Location = "Bangalore",
                    Notes = "Referred by alumni.", UpdatedAt = DateTime.Now.AddDays(-5)
                };
                var app2 = new JobApplication {
                    StudentId = student1.Id,
                    CompanyName = "Microsoft", Role = "Cloud Engineer",
                    Status = "Interview", AppliedDate = DateTime.Now.AddDays(-15),
                    JobLink = "https://careers.microsoft.com", Location = "Hyderabad",
                    UpdatedAt = DateTime.Now.AddDays(-2)
                };
                var app3 = new JobApplication {
                    StudentId = student1.Id,
                    CompanyName = "Amazon", Role = "SDE-1",
                    Status = "Applied", AppliedDate = DateTime.Now.AddDays(-5),
                    UpdatedAt = DateTime.Now.AddDays(-5)
                };
                context.JobApplications.AddRange(app1, app2, app3);
                await context.SaveChangesAsync();

                // Interview for app2
                context.InterviewSchedules.Add(new InterviewSchedule {
                    JobApplicationId = app2.Id,
                    InterviewType = "Technical",
                    InterviewDate = DateTime.Now.AddDays(3),
                    InterviewTime = new TimeSpan(10, 0, 0),
                    MeetingLink = "https://teams.microsoft.com/meet/abc123",
                    Outcome = "Pending"
                });

                // Faculty remark
                if (faculty != null)
                    context.FacultyRemarks.Add(new FacultyRemark {
                        FacultyId = faculty.Id,
                        JobApplicationId = app2.Id,
                        Comment = "Good progress Rahul! Prepare system design thoroughly for Microsoft round."
                    });

                // Faculty mapping
                if (faculty != null && !context.FacultyStudentMappings.Any(m => m.StudentId == student1.Id))
                    context.FacultyStudentMappings.Add(new FacultyStudentMapping {
                        FacultyId = faculty.Id, StudentId = student1.Id
                    });

                await context.SaveChangesAsync();
            }

            // ── Seed Internship Applications for Student 1 ──
            if (student1 != null && !context.InternshipApplications.Any(a => a.StudentId == student1.Id))
            {
                var intern1 = new InternshipApplication {
                    StudentId   = student1.Id,
                    CompanyName = "Flipkart",
                    Role        = "SDE Intern",
                    InternshipType = "Summer",
                    WorkMode    = "Hybrid",
                    Status      = "Completed",
                    AppliedDate = DateTime.Now.AddMonths(-5),
                    StartDate   = DateTime.Now.AddMonths(-3),
                    EndDate     = DateTime.Now.AddMonths(-1),
                    Stipend     = "25000",
                    Location    = "Bangalore",
                    CertificateReceived = true,
                    IsPPOConverted = false,
                    Notes       = "Worked on search ranking improvements.",
                    UpdatedAt   = DateTime.Now.AddMonths(-1)
                };
                var intern2 = new InternshipApplication {
                    StudentId   = student1.Id,
                    CompanyName = "Microsoft",
                    Role        = "Research Intern",
                    InternshipType = "Research",
                    WorkMode    = "Remote",
                    Status      = "Selected",
                    AppliedDate = DateTime.Now.AddDays(-20),
                    StartDate   = DateTime.Now.AddDays(10),
                    EndDate     = DateTime.Now.AddDays(70),
                    Stipend     = "40000",
                    Location    = "Remote",
                    Notes       = "NLP research internship with Azure AI team.",
                    UpdatedAt   = DateTime.Now.AddDays(-3)
                };
                var intern3 = new InternshipApplication {
                    StudentId   = student1.Id,
                    CompanyName = "DRDO",
                    Role        = "Project Trainee",
                    InternshipType = "Industrial Training",
                    WorkMode    = "On-Site",
                    Status      = "Applied",
                    AppliedDate = DateTime.Now.AddDays(-2),
                    Stipend     = "8000",
                    Location    = "Delhi",
                    Notes       = "8-week mandatory industrial training.",
                    UpdatedAt   = DateTime.Now.AddDays(-2)
                };
                context.InternshipApplications.AddRange(intern1, intern2, intern3);
                await context.SaveChangesAsync();

                // Interview round for intern2
                context.InternshipInterviews.Add(new InternshipInterview {
                    InternshipApplicationId = intern2.Id,
                    RoundType     = "Technical",
                    InterviewDate = DateTime.Now.AddDays(15),
                    InterviewTime = new TimeSpan(11, 30, 0),
                    MeetingLink   = "https://teams.microsoft.com/meet/intern123",
                    Outcome       = "Pending"
                });

                // Faculty remark on internship
                if (faculty != null)
                    context.FacultyRemarks.Add(new FacultyRemark {
                        FacultyId = faculty.Id,
                        InternshipApplicationId = intern2.Id,
                        Comment = "Excellent opportunity Rahul! Brush up on transformers and NLP fundamentals."
                    });

                await context.SaveChangesAsync();
            }

            // ── Seed Placement Applications for Student 2 ──
            if (student2 != null && !context.JobApplications.Any(a => a.StudentId == student2.Id))
            {
                var app = new JobApplication {
                    StudentId = student2.Id,
                    CompanyName = "Infosys", Role = "System Engineer",
                    Status = "Offer", AppliedDate = DateTime.Now.AddDays(-20),
                    Package = "3.6 LPA", UpdatedAt = DateTime.Now.AddDays(-3)
                };
                context.JobApplications.Add(app);
                if (faculty != null && !context.FacultyStudentMappings.Any(m => m.StudentId == student2.Id))
                    context.FacultyStudentMappings.Add(new FacultyStudentMapping {
                        FacultyId = faculty.Id, StudentId = student2.Id
                    });
                await context.SaveChangesAsync();
            }

            // ── Seed Internship Applications for Student 2 ──
            if (student2 != null && !context.InternshipApplications.Any(a => a.StudentId == student2.Id))
            {
                var intern = new InternshipApplication {
                    StudentId   = student2.Id,
                    CompanyName = "TCS",
                    Role        = "IT Analyst Intern",
                    InternshipType = "Winter",
                    WorkMode    = "On-Site",
                    Status      = "Completed",
                    AppliedDate = DateTime.Now.AddMonths(-4),
                    StartDate   = DateTime.Now.AddMonths(-3),
                    EndDate     = DateTime.Now.AddMonths(-2),
                    Stipend     = "10000",
                    Location    = "Pune",
                    CertificateReceived = true,
                    IsPPOConverted = true,
                    PPOPackage  = "3.6 LPA",
                    Notes       = "PPO extended after winter internship performance.",
                    UpdatedAt   = DateTime.Now.AddMonths(-2)
                };
                context.InternshipApplications.Add(intern);
                await context.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUser?> CreateUser(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user, string password, string role)
        {
            if (await userManager.FindByEmailAsync(user.Email!) != null) return null;
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
            return result.Succeeded ? user : null;
        }
    }
}
```

---

## ═══════════════════════════════
## SECTION 5 — SERVICES
## ═══════════════════════════════

### FILE: `Services/ApplicationService.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Services
{
    public class ApplicationService
    {
        private readonly AppDbContext _db;
        public ApplicationService(AppDbContext db) { _db = db; }

        public async Task<List<JobApplication>> GetStudentApplicationsAsync(
            string studentId, string? status = null, string? company = null,
            DateTime? from = null, DateTime? to = null)
        {
            var q = _db.JobApplications
                .Include(a => a.Interviews)
                .Include(a => a.Remarks).ThenInclude(r => r.Faculty)
                .Where(a => a.StudentId == studentId && a.IsActive);

            if (!string.IsNullOrEmpty(status)) q = q.Where(a => a.Status == status);
            if (!string.IsNullOrEmpty(company)) q = q.Where(a => a.CompanyName.Contains(company));
            if (from.HasValue) q = q.Where(a => a.AppliedDate >= from.Value);
            if (to.HasValue)   q = q.Where(a => a.AppliedDate <= to.Value);

            return await q.OrderByDescending(a => a.UpdatedAt).ToListAsync();
        }

        public async Task<JobApplication?> GetByIdAsync(int id)
            => await _db.JobApplications
                .Include(a => a.Student)
                .Include(a => a.Interviews)
                .Include(a => a.Remarks).ThenInclude(r => r.Faculty)
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

        public async Task<JobApplication> CreateAsync(JobApplication app)
        {
            _db.JobApplications.Add(app);
            await _db.SaveChangesAsync();
            return app;
        }

        public async Task<bool> UpdateAsync(JobApplication app)
        {
            app.UpdatedAt = DateTime.Now;
            _db.JobApplications.Update(app);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id, string studentId)
        {
            var app = await _db.JobApplications.FirstOrDefaultAsync(a => a.Id == id && a.StudentId == studentId);
            if (app == null) return false;
            app.IsActive = false;
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStatusAsync(int id, string studentId, string newStatus)
        {
            var app = await _db.JobApplications.FirstOrDefaultAsync(a => a.Id == id && a.StudentId == studentId);
            if (app == null) return false;
            app.Status = newStatus;
            app.UpdatedAt = DateTime.Now;
            return await _db.SaveChangesAsync() > 0;
        }

        // ── Interviews ──
        public async Task AddInterviewAsync(InterviewSchedule interview)
        {
            _db.InterviewSchedules.Add(interview);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteInterviewAsync(int interviewId)
        {
            var iv = await _db.InterviewSchedules.FindAsync(interviewId);
            if (iv == null) return false;
            _db.InterviewSchedules.Remove(iv);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateInterviewOutcomeAsync(int interviewId, string outcome)
        {
            var iv = await _db.InterviewSchedules.FindAsync(interviewId);
            if (iv == null) return false;
            iv.Outcome = outcome;
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
```

### FILE: `Services/InternshipService.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;

namespace PlacementTracker.Services
{
    /// <summary>
    /// Service for all internship application CRUD operations,
    /// filtering, and interview management.
    /// </summary>
    public class InternshipService
    {
        private readonly AppDbContext _db;
        public InternshipService(AppDbContext db) { _db = db; }

        // ── LIST with filters ──
        public async Task<List<InternshipApplication>> GetStudentInternshipsAsync(
            string studentId,
            string? status = null,
            string? company = null,
            string? internshipType = null,
            string? workMode = null,
            DateTime? from = null,
            DateTime? to = null)
        {
            var q = _db.InternshipApplications
                .Include(a => a.Interviews)
                .Include(a => a.Remarks).ThenInclude(r => r.Faculty)
                .Where(a => a.StudentId == studentId && a.IsActive);

            if (!string.IsNullOrEmpty(status))        q = q.Where(a => a.Status == status);
            if (!string.IsNullOrEmpty(company))       q = q.Where(a => a.CompanyName.Contains(company));
            if (!string.IsNullOrEmpty(internshipType))q = q.Where(a => a.InternshipType == internshipType);
            if (!string.IsNullOrEmpty(workMode))      q = q.Where(a => a.WorkMode == workMode);
            if (from.HasValue) q = q.Where(a => a.AppliedDate >= from.Value);
            if (to.HasValue)   q = q.Where(a => a.AppliedDate <= to.Value);

            return await q.OrderByDescending(a => a.UpdatedAt).ToListAsync();
        }

        // ── GET BY ID ──
        public async Task<InternshipApplication?> GetByIdAsync(int id)
            => await _db.InternshipApplications
                .Include(a => a.Student)
                .Include(a => a.Interviews)
                .Include(a => a.Remarks).ThenInclude(r => r.Faculty)
                .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

        // ── CREATE ──
        public async Task<InternshipApplication> CreateAsync(InternshipApplication app)
        {
            _db.InternshipApplications.Add(app);
            await _db.SaveChangesAsync();
            return app;
        }

        // ── UPDATE ──
        public async Task<bool> UpdateAsync(InternshipApplication app)
        {
            app.UpdatedAt = DateTime.Now;
            _db.InternshipApplications.Update(app);
            return await _db.SaveChangesAsync() > 0;
        }

        // ── SOFT DELETE ──
        public async Task<bool> DeleteAsync(int id, string studentId)
        {
            var app = await _db.InternshipApplications
                .FirstOrDefaultAsync(a => a.Id == id && a.StudentId == studentId);
            if (app == null) return false;
            app.IsActive = false;
            return await _db.SaveChangesAsync() > 0;
        }

        // ── QUICK STATUS UPDATE ──
        public async Task<bool> UpdateStatusAsync(int id, string studentId, string newStatus)
        {
            var app = await _db.InternshipApplications
                .FirstOrDefaultAsync(a => a.Id == id && a.StudentId == studentId);
            if (app == null) return false;
            app.Status = newStatus;
            app.UpdatedAt = DateTime.Now;
            return await _db.SaveChangesAsync() > 0;
        }

        // ── MARK PPO CONVERTED ──
        public async Task<bool> MarkPPOAsync(int id, string studentId, string? ppoPackage)
        {
            var app = await _db.InternshipApplications
                .FirstOrDefaultAsync(a => a.Id == id && a.StudentId == studentId);
            if (app == null) return false;
            app.IsPPOConverted = true;
            app.PPOPackage = ppoPackage;
            app.UpdatedAt = DateTime.Now;
            return await _db.SaveChangesAsync() > 0;
        }

        // ── MARK CERTIFICATE RECEIVED ──
        public async Task<bool> MarkCertificateAsync(int id, string studentId)
        {
            var app = await _db.InternshipApplications
                .FirstOrDefaultAsync(a => a.Id == id && a.StudentId == studentId);
            if (app == null) return false;
            app.CertificateReceived = true;
            app.UpdatedAt = DateTime.Now;
            return await _db.SaveChangesAsync() > 0;
        }

        // ── INTERVIEWS ──
        public async Task AddInterviewAsync(InternshipInterview interview)
        {
            _db.InternshipInterviews.Add(interview);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteInterviewAsync(int interviewId)
        {
            var iv = await _db.InternshipInterviews.FindAsync(interviewId);
            if (iv == null) return false;
            _db.InternshipInterviews.Remove(iv);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateInterviewOutcomeAsync(int interviewId, string outcome)
        {
            var iv = await _db.InternshipInterviews.FindAsync(interviewId);
            if (iv == null) return false;
            iv.Outcome = outcome;
            return await _db.SaveChangesAsync() > 0;
        }

        // ── ADMIN: ALL INTERNSHIPS with filters ──
        public async Task<List<InternshipApplication>> GetAllInternshipsAsync(
            string? status = null, string? department = null,
            string? company = null, string? internshipType = null)
        {
            var q = _db.InternshipApplications
                .Include(a => a.Student)
                .Where(a => a.IsActive).AsQueryable();

            if (!string.IsNullOrEmpty(status))        q = q.Where(a => a.Status == status);
            if (!string.IsNullOrEmpty(company))       q = q.Where(a => a.CompanyName.Contains(company));
            if (!string.IsNullOrEmpty(internshipType))q = q.Where(a => a.InternshipType == internshipType);
            if (!string.IsNullOrEmpty(department))    q = q.Where(a => a.Student!.Department == department);

            return await q.OrderByDescending(a => a.UpdatedAt).ToListAsync();
        }
    }
}
```

### FILE: `Services/AnalyticsService.cs`
```csharp
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Services
{
    public class AnalyticsService
    {
        private readonly AppDbContext _db;
        public AnalyticsService(AppDbContext db) { _db = db; }

        public async Task<DashboardViewModel> GetStudentDashboardAsync(string studentId)
        {
            // ── Placement apps ──
            var apps = await _db.JobApplications
                .Include(a => a.Interviews)
                .Where(a => a.StudentId == studentId && a.IsActive)
                .ToListAsync();

            // ── Internship apps ──
            var internships = await _db.InternshipApplications
                .Include(a => a.Interviews)
                .Where(a => a.StudentId == studentId && a.IsActive)
                .ToListAsync();

            var upcomingPlacement = apps.SelectMany(a => a.Interviews)
                .Where(i => i.InterviewDate >= DateTime.Today)
                .OrderBy(i => i.InterviewDate).Take(5).ToList();

            var upcomingInternship = internships.SelectMany(a => a.Interviews)
                .Where(i => i.InterviewDate >= DateTime.Today)
                .OrderBy(i => i.InterviewDate).Take(5).ToList();

            var notifs = await _db.Notifications
                .Where(n => n.UserId == studentId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt).Take(10).ToListAsync();

            return new DashboardViewModel
            {
                // Placement counts
                TotalApplications = apps.Count,
                Applied    = apps.Count(a => a.Status == "Applied"),
                Screening  = apps.Count(a => a.Status == "Screening"),
                Interview  = apps.Count(a => a.Status == "Interview"),
                Offer      = apps.Count(a => a.Status == "Offer"),
                Rejected   = apps.Count(a => a.Status == "Rejected"),
                Withdrawn  = apps.Count(a => a.Status == "Withdrawn"),

                // Internship counts
                TotalInternships        = internships.Count,
                InternshipApplied       = internships.Count(a => a.Status == "Applied"),
                InternshipShortlisted   = internships.Count(a => a.Status == "Shortlisted"),
                InternshipSelected      = internships.Count(a => a.Status == "Selected"),
                InternshipCompleted     = internships.Count(a => a.Status == "Completed"),
                InternshipRejected      = internships.Count(a => a.Status == "Rejected"),
                PPOConverted            = internships.Count(a => a.IsPPOConverted),
                HasOngoingInternship    = internships.Any(a => a.IsOngoing),

                // Upcoming
                UpcomingInterviews           = upcomingPlacement,
                UpcomingInternshipInterviews = upcomingInternship,
                RecentApplications = apps.OrderByDescending(a => a.UpdatedAt).Take(5).ToList(),
                RecentInternships  = internships.OrderByDescending(a => a.UpdatedAt).Take(5).ToList(),
                Notifications = notifs
            };
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardAsync()
        {
            var apps        = await _db.JobApplications.Include(a=>a.Student).Where(a => a.IsActive).ToListAsync();
            var internships = await _db.InternshipApplications.Include(a=>a.Student).Where(a => a.IsActive).ToListAsync();
            var users       = await _db.Users.ToListAsync();

            var roleMap = (await _db.UserRoles.ToListAsync())
                .Join(await _db.Roles.ToListAsync(),
                    ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name });
            var studentIds = roleMap.Where(x => x.Name == "Student").Select(x => x.UserId).ToHashSet();
            var facultyIds = roleMap.Where(x => x.Name == "Faculty").Select(x => x.UserId).ToHashSet();

            var offers     = apps.Where(a => a.Status == "Offer").ToList();
            var internSelected  = internships.Where(a => a.Status == "Selected" || a.Status == "Completed").ToList();
            int totalStudents   = studentIds.Count;

            return new AdminDashboardViewModel
            {
                TotalStudents     = totalStudents,
                TotalFaculty      = facultyIds.Count,
                TotalApplications = apps.Count,
                TotalOffers       = offers.Count,
                TotalInterviews   = apps.Count(a => a.Status == "Interview"),
                PlacementRate     = totalStudents > 0
                    ? Math.Round((double)offers.Count / totalStudents * 100, 1) : 0,

                TotalInternships         = internships.Count,
                TotalInternshipSelected  = internships.Count(a => a.Status == "Selected"),
                TotalInternshipCompleted = internships.Count(a => a.Status == "Completed"),
                TotalPPOConverted        = internships.Count(a => a.IsPPOConverted),
                InternshipConversionRate = internships.Count > 0
                    ? Math.Round((double)internships.Count(a => a.IsPPOConverted) / internships.Count * 100, 1) : 0,

                StatusDistribution = new Dictionary<string, int>
                {
                    ["Applied"]   = apps.Count(a => a.Status == "Applied"),
                    ["Screening"] = apps.Count(a => a.Status == "Screening"),
                    ["Interview"] = apps.Count(a => a.Status == "Interview"),
                    ["Offer"]     = apps.Count(a => a.Status == "Offer"),
                    ["Rejected"]  = apps.Count(a => a.Status == "Rejected"),
                },

                InternshipStatusDistribution = new Dictionary<string, int>
                {
                    ["Applied"]     = internships.Count(a => a.Status == "Applied"),
                    ["Shortlisted"] = internships.Count(a => a.Status == "Shortlisted"),
                    ["Selected"]    = internships.Count(a => a.Status == "Selected"),
                    ["Completed"]   = internships.Count(a => a.Status == "Completed"),
                    ["Rejected"]    = internships.Count(a => a.Status == "Rejected"),
                },

                InternshipTypeDistribution = internships
                    .GroupBy(a => a.InternshipType)
                    .ToDictionary(g => g.Key, g => g.Count()),

                TopCompanies = apps.GroupBy(a => a.CompanyName)
                    .OrderByDescending(g => g.Count()).Take(5)
                    .ToDictionary(g => g.Key, g => g.Count()),

                TopInternshipCompanies = internships.GroupBy(a => a.CompanyName)
                    .OrderByDescending(g => g.Count()).Take(5)
                    .ToDictionary(g => g.Key, g => g.Count()),

                RecentActivity = apps.OrderByDescending(a => a.UpdatedAt).Take(10)
                    .Select(a => new RecentActivityItem {
                        StudentName = a.Student?.FullName ?? "—",
                        CompanyName = a.CompanyName,
                        Status      = a.Status,
                        UpdatedAt   = a.UpdatedAt
                    }).ToList(),

                RecentInternshipActivity = internships.OrderByDescending(a => a.UpdatedAt).Take(10)
                    .Select(a => new RecentInternshipActivityItem {
                        StudentName    = a.Student?.FullName ?? "—",
                        CompanyName    = a.CompanyName,
                        InternshipType = a.InternshipType,
                        Status         = a.Status,
                        UpdatedAt      = a.UpdatedAt
                    }).ToList()
            };
        }
    }
}
```

### FILE: `Services/NotificationService.cs`
```csharp
using PlacementTracker.Data;
using PlacementTracker.Models;

namespace PlacementTracker.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _db;
        public NotificationService(AppDbContext db) { _db = db; }

        public async Task SendAsync(string userId, string message, string? link = null)
        {
            _db.Notifications.Add(new Notification {
                UserId = userId, Message = message, Link = link
            });
            await _db.SaveChangesAsync();
        }

        public async Task MarkReadAsync(int id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n != null) { n.IsRead = true; await _db.SaveChangesAsync(); }
        }
    }
}
```

---

## ═══════════════════════════════
## SECTION 6 — CONTROLLERS
## ═══════════════════════════════

### FILE: `Controllers/AccountController.cs`
```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlacementTracker.Models;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> u,
            SignInManager<ApplicationUser> s, RoleManager<IdentityRole> r)
        { _userManager = u; _signInManager = s; _roleManager = r; }

        [HttpGet] public IActionResult Login() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            { ModelState.AddModelError("", "Invalid credentials."); return View(model); }

            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe, false);

            if (!result.Succeeded)
            { ModelState.AddModelError("", "Invalid credentials."); return View(model); }

            var roles = await _userManager.GetRolesAsync(user);
            var role  = roles.FirstOrDefault();
            return role switch {
                "SuperAdmin" => RedirectToAction("Index", "SuperAdmin"),
                "Admin"      => RedirectToAction("Index", "Admin"),
                "Faculty"    => RedirectToAction("Index", "Faculty"),
                _            => RedirectToAction("Index", "Dashboard")
            };
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        { await _signInManager.SignOutAsync(); return RedirectToAction("Login"); }

        [HttpGet] public IActionResult Register() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser {
                UserName = model.Email, Email = model.Email,
                FullName = model.FullName, Department = model.Department,
                CollegeRollNo = model.CollegeRollNo, Phone = model.Phone,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                TempData["Success"] = "Account created successfully!";
                return RedirectToAction("Login");
            }
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        public IActionResult AccessDenied() => View();
    }
}
```

### FILE: `Controllers/DashboardController.cs`
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlacementTracker.Models;
using PlacementTracker.Services;

namespace PlacementTracker.Controllers
{
    [Authorize(Roles = "Student")]
    public class DashboardController : Controller
    {
        private readonly AnalyticsService _analytics;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(AnalyticsService a, UserManager<ApplicationUser> u)
        { _analytics = a; _userManager = u; }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var vm = await _analytics.GetStudentDashboardAsync(user!.Id);
            return View(vm);
        }
    }
}
```

### FILE: `Controllers/ApplicationsController.cs`
```csharp
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlacementTracker.Models;
using PlacementTracker.Services;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Controllers
{
    [Authorize(Roles = "Student")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationService _svc;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationsController(ApplicationService s, UserManager<ApplicationUser> u)
        { _svc = s; _userManager = u; }

        private async Task<string> GetUserId()
            => (await _userManager.GetUserAsync(User))!.Id;

        public async Task<IActionResult> Index(
            string? status, string? company, DateTime? from, DateTime? to)
        {
            var id = await GetUserId();
            var apps = await _svc.GetStudentApplicationsAsync(id, status, company, from, to);
            ViewBag.Filter_Status  = status;
            ViewBag.Filter_Company = company;
            ViewBag.Filter_From    = from?.ToString("yyyy-MM-dd");
            ViewBag.Filter_To      = to?.ToString("yyyy-MM-dd");
            ViewBag.StatusList     = ApplicationFormViewModel.StatusList;
            return View(apps);
        }

        [HttpGet] public IActionResult Create()
        {
            ViewBag.StatusList = ApplicationFormViewModel.StatusList;
            return View(new ApplicationFormViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationFormViewModel model)
        {
            if (!ModelState.IsValid)
            { ViewBag.StatusList = ApplicationFormViewModel.StatusList; return View(model); }

            var app = new JobApplication {
                StudentId   = await GetUserId(),
                CompanyName = model.CompanyName, Role = model.Role,
                Status      = model.Status, AppliedDate = model.AppliedDate,
                JobLink     = model.JobLink, Notes = model.Notes,
                Location    = model.Location, Package = model.Package
            };
            await _svc.CreateAsync(app);
            TempData["Success"] = $"Application to {app.CompanyName} added!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet] public async Task<IActionResult> Edit(int id)
        {
            var app = await _svc.GetByIdAsync(id);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();
            ViewBag.StatusList = ApplicationFormViewModel.StatusList;
            return View(new ApplicationFormViewModel {
                Id = app.Id, CompanyName = app.CompanyName, Role = app.Role,
                Status = app.Status, AppliedDate = app.AppliedDate,
                JobLink = app.JobLink, Notes = app.Notes,
                Location = app.Location, Package = app.Package
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ApplicationFormViewModel model)
        {
            if (!ModelState.IsValid)
            { ViewBag.StatusList = ApplicationFormViewModel.StatusList; return View(model); }

            var app = await _svc.GetByIdAsync(id);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();
            app.CompanyName = model.CompanyName; app.Role = model.Role;
            app.Status = model.Status; app.AppliedDate = model.AppliedDate;
            app.JobLink = model.JobLink; app.Notes = model.Notes;
            app.Location = model.Location; app.Package = model.Package;
            await _svc.UpdateAsync(app);
            TempData["Success"] = "Application updated!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var app = await _svc.GetByIdAsync(id);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();
            return View(app);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id, await GetUserId());
            TempData["Success"] = "Application removed.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _svc.UpdateStatusAsync(id, await GetUserId(), status);
            TempData["Success"] = $"Status updated to {status}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInterview(int appId,
            string interviewType, DateTime interviewDate,
            TimeSpan? interviewTime, string? meetingLink, string? venue, string? notes)
        {
            var app = await _svc.GetByIdAsync(appId);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();
            await _svc.AddInterviewAsync(new InterviewSchedule {
                JobApplicationId = appId, InterviewType = interviewType,
                InterviewDate = interviewDate, InterviewTime = interviewTime,
                MeetingLink = meetingLink, Venue = venue, Notes = notes, Outcome = "Pending"
            });
            TempData["Success"] = "Interview scheduled!";
            return RedirectToAction(nameof(Details), new { id = appId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInterview(int interviewId, int appId)
        {
            await _svc.DeleteInterviewAsync(interviewId);
            return RedirectToAction(nameof(Details), new { id = appId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInterviewOutcome(
            int interviewId, int appId, string outcome)
        {
            await _svc.UpdateInterviewOutcomeAsync(interviewId, outcome);
            return RedirectToAction(nameof(Details), new { id = appId });
        }

        public async Task<IActionResult> ExportExcel()
        {
            var id   = await GetUserId();
            var apps = await _svc.GetStudentApplicationsAsync(id);
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Placements");
            string[] headers = { "Company","Role","Status","Applied Date","Location","Package" };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i+1).Value = headers[i];
            int row = 2;
            foreach (var a in apps)
            {
                ws.Cell(row,1).Value = a.CompanyName;
                ws.Cell(row,2).Value = a.Role;
                ws.Cell(row,3).Value = a.Status;
                ws.Cell(row,4).Value = a.AppliedDate.ToString("dd-MM-yyyy");
                ws.Cell(row,5).Value = a.Location ?? "-";
                ws.Cell(row,6).Value = a.Package ?? "-";
                row++;
            }
            ws.Row(1).Style.Font.Bold = true;
            ws.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "MyPlacementApplications.xlsx");
        }
    }
}
```

### FILE: `Controllers/InternshipsController.cs`
```csharp
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlacementTracker.Models;
using PlacementTracker.Services;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Controllers
{
    /// <summary>
    /// Student-facing controller for full internship application lifecycle:
    /// list, create, edit, details, delete, interview scheduling, PPO marking, export.
    /// </summary>
    [Authorize(Roles = "Student")]
    public class InternshipsController : Controller
    {
        private readonly InternshipService _svc;
        private readonly UserManager<ApplicationUser> _userManager;

        public InternshipsController(InternshipService s, UserManager<ApplicationUser> u)
        { _svc = s; _userManager = u; }

        private async Task<string> GetUserId()
            => (await _userManager.GetUserAsync(User))!.Id;

        // ── LIST with filters ──
        public async Task<IActionResult> Index(
            string? status, string? company,
            string? internshipType, string? workMode,
            DateTime? from, DateTime? to)
        {
            var id = await GetUserId();
            var internships = await _svc.GetStudentInternshipsAsync(
                id, status, company, internshipType, workMode, from, to);

            ViewBag.Filter_Status        = status;
            ViewBag.Filter_Company       = company;
            ViewBag.Filter_InternshipType= internshipType;
            ViewBag.Filter_WorkMode      = workMode;
            ViewBag.Filter_From          = from?.ToString("yyyy-MM-dd");
            ViewBag.Filter_To            = to?.ToString("yyyy-MM-dd");
            ViewBag.StatusList           = InternshipFormViewModel.StatusList;
            ViewBag.TypeList             = InternshipFormViewModel.TypeList;
            ViewBag.ModeList             = InternshipFormViewModel.ModeList;
            return View(internships);
        }

        // ── CREATE ──
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.StatusList = InternshipFormViewModel.StatusList;
            ViewBag.TypeList   = InternshipFormViewModel.TypeList;
            ViewBag.ModeList   = InternshipFormViewModel.ModeList;
            return View(new InternshipFormViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InternshipFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = InternshipFormViewModel.StatusList;
                ViewBag.TypeList   = InternshipFormViewModel.TypeList;
                ViewBag.ModeList   = InternshipFormViewModel.ModeList;
                return View(model);
            }
            var app = new InternshipApplication {
                StudentId      = await GetUserId(),
                CompanyName    = model.CompanyName,
                Role           = model.Role,
                InternshipType = model.InternshipType,
                WorkMode       = model.WorkMode,
                Status         = model.Status,
                AppliedDate    = model.AppliedDate,
                StartDate      = model.StartDate,
                EndDate        = model.EndDate,
                Stipend        = model.Stipend,
                Location       = model.Location,
                JobLink        = model.JobLink,
                Notes          = model.Notes,
                IsPPOConverted  = model.IsPPOConverted,
                PPOPackage     = model.PPOPackage,
                CertificateReceived = model.CertificateReceived
            };
            await _svc.CreateAsync(app);
            TempData["Success"] = $"Internship at {app.CompanyName} added!";
            return RedirectToAction(nameof(Index));
        }

        // ── EDIT ──
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var app = await _svc.GetByIdAsync(id);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();
            ViewBag.StatusList = InternshipFormViewModel.StatusList;
            ViewBag.TypeList   = InternshipFormViewModel.TypeList;
            ViewBag.ModeList   = InternshipFormViewModel.ModeList;
            return View(new InternshipFormViewModel {
                Id = app.Id, CompanyName = app.CompanyName, Role = app.Role,
                InternshipType = app.InternshipType, WorkMode = app.WorkMode,
                Status = app.Status, AppliedDate = app.AppliedDate,
                StartDate = app.StartDate, EndDate = app.EndDate,
                Stipend = app.Stipend, Location = app.Location,
                JobLink = app.JobLink, Notes = app.Notes,
                IsPPOConverted = app.IsPPOConverted, PPOPackage = app.PPOPackage,
                CertificateReceived = app.CertificateReceived
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InternshipFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = InternshipFormViewModel.StatusList;
                ViewBag.TypeList   = InternshipFormViewModel.TypeList;
                ViewBag.ModeList   = InternshipFormViewModel.ModeList;
                return View(model);
            }
            var app = await _svc.GetByIdAsync(id);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();

            app.CompanyName    = model.CompanyName; app.Role = model.Role;
            app.InternshipType = model.InternshipType; app.WorkMode = model.WorkMode;
            app.Status = model.Status; app.AppliedDate = model.AppliedDate;
            app.StartDate = model.StartDate; app.EndDate = model.EndDate;
            app.Stipend = model.Stipend; app.Location = model.Location;
            app.JobLink = model.JobLink; app.Notes = model.Notes;
            app.IsPPOConverted = model.IsPPOConverted; app.PPOPackage = model.PPOPackage;
            app.CertificateReceived = model.CertificateReceived;

            await _svc.UpdateAsync(app);
            TempData["Success"] = "Internship application updated!";
            return RedirectToAction(nameof(Index));
        }

        // ── DETAILS ──
        public async Task<IActionResult> Details(int id)
        {
            var app = await _svc.GetByIdAsync(id);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();
            return View(app);
        }

        // ── DELETE ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _svc.DeleteAsync(id, await GetUserId());
            TempData["Success"] = "Internship application removed.";
            return RedirectToAction(nameof(Index));
        }

        // ── QUICK STATUS UPDATE ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _svc.UpdateStatusAsync(id, await GetUserId(), status);
            TempData["Success"] = $"Status updated to {status}.";
            return RedirectToAction(nameof(Index));
        }

        // ── MARK PPO CONVERTED ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPPO(int id, string? ppoPackage)
        {
            await _svc.MarkPPOAsync(id, await GetUserId(), ppoPackage);
            TempData["Success"] = "Marked as PPO converted! 🎉";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ── MARK CERTIFICATE RECEIVED ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkCertificate(int id)
        {
            await _svc.MarkCertificateAsync(id, await GetUserId());
            TempData["Success"] = "Certificate marked as received!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ── ADD INTERVIEW ROUND ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInterview(int appId,
            string roundType, DateTime interviewDate,
            TimeSpan? interviewTime, string? meetingLink, string? venue, string? notes)
        {
            var app = await _svc.GetByIdAsync(appId);
            if (app == null || app.StudentId != await GetUserId()) return Forbid();
            await _svc.AddInterviewAsync(new InternshipInterview {
                InternshipApplicationId = appId, RoundType = roundType,
                InterviewDate = interviewDate, InterviewTime = interviewTime,
                MeetingLink = meetingLink, Venue = venue, Notes = notes, Outcome = "Pending"
            });
            TempData["Success"] = "Interview round scheduled!";
            return RedirectToAction(nameof(Details), new { id = appId });
        }

        // ── DELETE INTERVIEW ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInterview(int interviewId, int appId)
        {
            await _svc.DeleteInterviewAsync(interviewId);
            return RedirectToAction(nameof(Details), new { id = appId });
        }

        // ── UPDATE INTERVIEW OUTCOME ──
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInterviewOutcome(
            int interviewId, int appId, string outcome)
        {
            await _svc.UpdateInterviewOutcomeAsync(interviewId, outcome);
            return RedirectToAction(nameof(Details), new { id = appId });
        }

        // ── EXPORT TO EXCEL ──
        public async Task<IActionResult> ExportExcel()
        {
            var id   = await GetUserId();
            var list = await _svc.GetStudentInternshipsAsync(id);
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Internships");
            string[] headers = {
                "Company","Role","Type","Mode","Status",
                "Applied Date","Start Date","End Date",
                "Stipend/Month","PPO","PPO Package","Certificate","Location","Notes"
            };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i+1).Value = headers[i];
            int row = 2;
            foreach (var a in list)
            {
                ws.Cell(row, 1).Value  = a.CompanyName;
                ws.Cell(row, 2).Value  = a.Role;
                ws.Cell(row, 3).Value  = a.InternshipType;
                ws.Cell(row, 4).Value  = a.WorkMode;
                ws.Cell(row, 5).Value  = a.Status;
                ws.Cell(row, 6).Value  = a.AppliedDate.ToString("dd-MM-yyyy");
                ws.Cell(row, 7).Value  = a.StartDate?.ToString("dd-MM-yyyy") ?? "-";
                ws.Cell(row, 8).Value  = a.EndDate?.ToString("dd-MM-yyyy") ?? "-";
                ws.Cell(row, 9).Value  = a.Stipend != null ? $"₹{a.Stipend}" : "-";
                ws.Cell(row, 10).Value = a.IsPPOConverted ? "Yes" : "No";
                ws.Cell(row, 11).Value = a.PPOPackage ?? "-";
                ws.Cell(row, 12).Value = a.CertificateReceived ? "Yes" : "No";
                ws.Cell(row, 13).Value = a.Location ?? "-";
                ws.Cell(row, 14).Value = a.Notes ?? "-";
                row++;
            }
            ws.Row(1).Style.Font.Bold = true;
            ws.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "MyInternships.xlsx");
        }
    }
}
```

### FILE: `Controllers/FacultyController.cs`
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public FacultyController(AppDbContext db, UserManager<ApplicationUser> u)
        { _db = db; _userManager = u; }

        private async Task<string> GetFacultyId()
            => (await _userManager.GetUserAsync(User))!.Id;

        public async Task<IActionResult> Index()
        {
            var facultyId = await GetFacultyId();
            var studentIds = await _db.FacultyStudentMappings
                .Where(m => m.FacultyId == facultyId)
                .Select(m => m.StudentId).ToListAsync();

            var students = await _db.Users
                .Where(u => studentIds.Contains(u.Id)).ToListAsync();

            var items = new List<StudentProgressItem>();
            foreach (var s in students)
            {
                var apps = await _db.JobApplications
                    .Where(a => a.StudentId == s.Id && a.IsActive).ToListAsync();
                var interns = await _db.InternshipApplications
                    .Where(a => a.StudentId == s.Id && a.IsActive).ToListAsync();

                items.Add(new StudentProgressItem {
                    Student             = s,
                    TotalApps           = apps.Count,
                    Offers              = apps.Count(a => a.Status == "Offer"),
                    Interviews          = apps.Count(a => a.Status == "Interview"),
                    LatestStatus        = apps.OrderByDescending(a => a.UpdatedAt)
                                             .FirstOrDefault()?.Status ?? "—",
                    TotalInternships    = interns.Count,
                    InternshipSelected  = interns.Count(a => a.Status == "Selected" || a.Status == "Completed"),
                    HasPPO              = interns.Any(a => a.IsPPOConverted),
                    HasOngoingInternship = interns.Any(a => a.IsOngoing)
                });
            }

            var upcomingPlacement = await _db.InterviewSchedules
                .Include(i => i.JobApplication).ThenInclude(a => a!.Student)
                .Where(i => studentIds.Contains(i.JobApplication!.StudentId)
                    && i.InterviewDate >= DateTime.Today)
                .OrderBy(i => i.InterviewDate).Take(10).ToListAsync();

            var upcomingInternship = await _db.InternshipInterviews
                .Include(i => i.InternshipApplication).ThenInclude(a => a!.Student)
                .Where(i => studentIds.Contains(i.InternshipApplication!.StudentId)
                    && i.InterviewDate >= DateTime.Today)
                .OrderBy(i => i.InterviewDate).Take(10).ToListAsync();

            return View(new FacultyDashboardViewModel {
                Students                   = items,
                UpcomingInterviews         = upcomingPlacement,
                UpcomingInternshipInterviews = upcomingInternship,
                TotalStudents              = students.Count,
                TotalApplications          = items.Sum(i => i.TotalApps),
                TotalOffers                = items.Sum(i => i.Offers),
                TotalInternships           = items.Sum(i => i.TotalInternships),
                TotalInternshipSelected    = items.Sum(i => i.InternshipSelected),
                TotalPPOConverted          = items.Count(i => i.HasPPO)
            });
        }

        public async Task<IActionResult> StudentDetails(string studentId)
        {
            var facultyId = await GetFacultyId();
            if (!await _db.FacultyStudentMappings.AnyAsync(
                m => m.FacultyId == facultyId && m.StudentId == studentId))
                return Forbid();

            var student = await _db.Users.FindAsync(studentId);
            var placements = await _db.JobApplications
                .Include(a => a.Remarks).ThenInclude(r => r.Faculty)
                .Include(a => a.Interviews)
                .Where(a => a.StudentId == studentId && a.IsActive).ToListAsync();
            var internships = await _db.InternshipApplications
                .Include(a => a.Remarks).ThenInclude(r => r.Faculty)
                .Include(a => a.Interviews)
                .Where(a => a.StudentId == studentId && a.IsActive).ToListAsync();

            ViewBag.Student    = student;
            ViewBag.Placements = placements;
            ViewBag.Internships= internships;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRemark(string studentId, int? appId,
            int? internshipId, string comment)
        {
            var facultyId = await GetFacultyId();
            _db.FacultyRemarks.Add(new FacultyRemark {
                FacultyId              = facultyId,
                JobApplicationId       = appId,
                InternshipApplicationId= internshipId,
                Comment                = comment
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Remark added!";
            return RedirectToAction(nameof(StudentDetails), new { studentId });
        }
    }
}
```

### FILE: `Controllers/AdminController.cs`
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;
using PlacementTracker.Services;
using PlacementTracker.ViewModels;

namespace PlacementTracker.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AnalyticsService _analytics;
        private readonly InternshipService _internshipSvc;

        public AdminController(AppDbContext db,
            UserManager<ApplicationUser> u, AnalyticsService a, InternshipService i)
        { _db = db; _userManager = u; _analytics = a; _internshipSvc = i; }

        public async Task<IActionResult> Index()
            => View(await _analytics.GetAdminDashboardAsync());

        // ── ALL PLACEMENTS ──
        public async Task<IActionResult> Applications(
            string? status, string? department, string? company)
        {
            var q = _db.JobApplications
                .Include(a => a.Student)
                .Where(a => a.IsActive).AsQueryable();

            if (!string.IsNullOrEmpty(status))    q = q.Where(a => a.Status == status);
            if (!string.IsNullOrEmpty(company))   q = q.Where(a => a.CompanyName.Contains(company));
            if (!string.IsNullOrEmpty(department)) q = q.Where(a => a.Student!.Department == department);

            ViewBag.StatusList  = ApplicationFormViewModel.StatusList;
            ViewBag.Departments = await _db.Users
                .Where(u => u.Department != null).Select(u => u.Department!).Distinct().ToListAsync();
            return View(await q.OrderByDescending(a => a.UpdatedAt).ToListAsync());
        }

        // ── ALL INTERNSHIPS (Admin View) ──
        public async Task<IActionResult> Internships(
            string? status, string? department, string? company, string? internshipType)
        {
            var list = await _internshipSvc.GetAllInternshipsAsync(status, department, company, internshipType);

            ViewBag.StatusList  = InternshipFormViewModel.StatusList;
            ViewBag.TypeList    = InternshipFormViewModel.TypeList;
            ViewBag.Departments = await _db.Users
                .Where(u => u.Department != null).Select(u => u.Department!).Distinct().ToListAsync();
            ViewBag.Filter_Status        = status;
            ViewBag.Filter_Company       = company;
            ViewBag.Filter_Department    = department;
            ViewBag.Filter_InternshipType= internshipType;
            return View(list);
        }

        // ── EXPORT ALL INTERNSHIPS ──
        public async Task<IActionResult> ExportInternshipsExcel()
        {
            var list = await _internshipSvc.GetAllInternshipsAsync();
            using var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.Worksheets.Add("All Internships");
            string[] headers = {
                "Student","Roll No","Department","Company","Role",
                "Type","Mode","Status","Stipend","PPO","PPO Package",
                "Certificate","Start Date","End Date"
            };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i+1).Value = headers[i];
            int row = 2;
            foreach (var a in list)
            {
                ws.Cell(row, 1).Value  = a.Student?.FullName ?? "-";
                ws.Cell(row, 2).Value  = a.Student?.CollegeRollNo ?? "-";
                ws.Cell(row, 3).Value  = a.Student?.Department ?? "-";
                ws.Cell(row, 4).Value  = a.CompanyName;
                ws.Cell(row, 5).Value  = a.Role;
                ws.Cell(row, 6).Value  = a.InternshipType;
                ws.Cell(row, 7).Value  = a.WorkMode;
                ws.Cell(row, 8).Value  = a.Status;
                ws.Cell(row, 9).Value  = a.Stipend != null ? $"₹{a.Stipend}" : "-";
                ws.Cell(row,10).Value  = a.IsPPOConverted ? "Yes" : "No";
                ws.Cell(row,11).Value  = a.PPOPackage ?? "-";
                ws.Cell(row,12).Value  = a.CertificateReceived ? "Yes" : "No";
                ws.Cell(row,13).Value  = a.StartDate?.ToString("dd-MM-yyyy") ?? "-";
                ws.Cell(row,14).Value  = a.EndDate?.ToString("dd-MM-yyyy") ?? "-";
                row++;
            }
            ws.Row(1).Style.Font.Bold = true;
            ws.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "InternshipReport.xlsx");
        }

        // ── USERS ──
        public async Task<IActionResult> Users()
        {
            ViewBag.Students = (await _userManager.GetUsersInRoleAsync("Student")).OrderBy(s => s.FullName).ToList();
            ViewBag.Faculty  = (await _userManager.GetUsersInRoleAsync("Faculty")).OrderBy(f => f.FullName).ToList();
            return View();
        }

        [HttpGet] public IActionResult CreateStudent()
            => View(new RegisterViewModel { Role = "Student" });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(RegisterViewModel model)
        {
            model.Role = "Student";
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser {
                UserName = model.Email, Email = model.Email,
                FullName = model.FullName, Department = model.Department,
                CollegeRollNo = model.CollegeRollNo, Phone = model.Phone,
                EmailConfirmed = true
            };
            var r = await _userManager.CreateAsync(user, model.Password);
            if (r.Succeeded)
            { await _userManager.AddToRoleAsync(user, "Student");
              TempData["Success"] = "Student created!"; return RedirectToAction(nameof(Users)); }
            foreach (var e in r.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        [HttpGet] public IActionResult CreateFaculty()
            => View(new RegisterViewModel { Role = "Faculty" });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFaculty(RegisterViewModel model)
        {
            model.Role = "Faculty";
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser {
                UserName = model.Email, Email = model.Email,
                FullName = model.FullName, Department = model.Department,
                EmailConfirmed = true
            };
            var r = await _userManager.CreateAsync(user, model.Password);
            if (r.Succeeded)
            { await _userManager.AddToRoleAsync(user, "Faculty");
              TempData["Success"] = "Faculty created!"; return RedirectToAction(nameof(Users)); }
            foreach (var e in r.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null) { user.IsActive = !user.IsActive; await _userManager.UpdateAsync(user); }
            TempData["Success"] = "User status updated.";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStudentToFaculty(string facultyId, string studentId)
        {
            if (!_db.FacultyStudentMappings.Any(m => m.FacultyId == facultyId && m.StudentId == studentId))
            {
                _db.FacultyStudentMappings.Add(new FacultyStudentMapping {
                    FacultyId = facultyId, StudentId = studentId });
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "Student assigned!";
            return RedirectToAction(nameof(Users));
        }

        // ── EXPORT ALL PLACEMENTS ──
        public async Task<IActionResult> ExportExcel()
        {
            using var wb = new ClosedXML.Excel.XLWorkbook();
            var ws = wb.Worksheets.Add("All Placements");
            string[] headers = { "Student","Roll No","Department","Company","Role","Status","Applied Date","Package","Location" };
            for (int i = 0; i < headers.Length; i++) ws.Cell(1, i+1).Value = headers[i];
            var apps = await _db.JobApplications
                .Include(a => a.Student).Where(a => a.IsActive)
                .OrderByDescending(a => a.UpdatedAt).ToListAsync();
            int row = 2;
            foreach (var a in apps)
            {
                ws.Cell(row,1).Value = a.Student?.FullName ?? "-";
                ws.Cell(row,2).Value = a.Student?.CollegeRollNo ?? "-";
                ws.Cell(row,3).Value = a.Student?.Department ?? "-";
                ws.Cell(row,4).Value = a.CompanyName;
                ws.Cell(row,5).Value = a.Role;
                ws.Cell(row,6).Value = a.Status;
                ws.Cell(row,7).Value = a.AppliedDate.ToString("dd-MM-yyyy");
                ws.Cell(row,8).Value = a.Package ?? "-";
                ws.Cell(row,9).Value = a.Location ?? "-";
                row++;
            }
            ws.Row(1).Style.Font.Bold = true;
            ws.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "PlacementReport.xlsx");
        }

        // ── COMPANIES ──
        public async Task<IActionResult> Companies()
            => View(await _db.Companies.OrderBy(c => c.Name).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCompany(string name, string? location,
            string? industry, string? website, bool offersInternships, bool offersPlacement)
        {
            _db.Companies.Add(new Company {
                Name = name, Location = location, Industry = industry,
                Website = website, OffersInternships = offersInternships,
                OffersPlacement = offersPlacement });
            await _db.SaveChangesAsync();
            TempData["Success"] = "Company added!";
            return RedirectToAction(nameof(Companies));
        }
    }
}
```

### FILE: `Controllers/SuperAdminController.cs`
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;

namespace PlacementTracker.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SuperAdminController(AppDbContext db,
            UserManager<ApplicationUser> u, RoleManager<IdentityRole> r)
        { _db = db; _userManager = u; _roleManager = r; }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers        = await _db.Users.CountAsync();
            ViewBag.TotalPlacements   = await _db.JobApplications.CountAsync(a => a.IsActive);
            ViewBag.TotalInternships  = await _db.InternshipApplications.CountAsync(a => a.IsActive);
            ViewBag.TotalOffers       = await _db.JobApplications.CountAsync(a => a.Status == "Offer" && a.IsActive);
            ViewBag.TotalPPO          = await _db.InternshipApplications.CountAsync(a => a.IsPPOConverted && a.IsActive);
            ViewBag.TotalCompanies    = await _db.Companies.CountAsync(c => c.IsActive);
            ViewBag.AllUsers          = await _db.Users.OrderBy(u => u.FullName).ToListAsync();
            return View();
        }
    }
}
```

---

## ═══════════════════════════════
## SECTION 7 — PROGRAM.CS
## ═══════════════════════════════

### FILE: `Program.cs`
```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;
using PlacementTracker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit           = true;
    options.Password.RequiredLength         = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase       = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath       = "/Account/Login";
    options.AccessDeniedPath= "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan  = TimeSpan.FromHours(8);
});

// ── Register Services ──
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<InternshipService>();      // ← NEW
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<NotificationService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ── Seed Database ──
using (var scope = app.Services.CreateScope())
    await SeedData.InitializeAsync(scope.ServiceProvider);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
```

---

## ═══════════════════════════════
## SECTION 8 — VIEWS
## ═══════════════════════════════

### FILE: `Views/Shared/_Layout.cshtml`
```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>@ViewData["Title"] — PlacementHub</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css"/>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css"/>
    <style>
        :root { --sidebar-w: 240px; }
        body { background: #f4f6fb; }
        .sidebar {
            width: var(--sidebar-w); min-height: 100vh;
            background: linear-gradient(160deg, #1a1f36 0%, #2d3561 100%);
            position: fixed; top: 0; left: 0; z-index: 100;
            display: flex; flex-direction: column;
        }
        .sidebar .brand { padding: 1.4rem 1.2rem; border-bottom: 1px solid rgba(255,255,255,.1); }
        .sidebar .brand span { color: #fff; font-weight: 700; font-size: 1.15rem; }
        .sidebar .nav-link {
            color: rgba(255,255,255,.7); padding: .55rem 1.2rem;
            border-radius: .4rem; margin: 1px 8px; font-size: .88rem;
            transition: all .2s;
        }
        .sidebar .nav-link:hover, .sidebar .nav-link.active {
            background: rgba(255,255,255,.15); color: #fff;
        }
        .sidebar .nav-link i { width: 20px; }
        .sidebar .section-label {
            color: rgba(255,255,255,.35); font-size: .7rem;
            text-transform: uppercase; letter-spacing: 1px;
            padding: .8rem 1.4rem .2rem;
        }
        .main-wrap { margin-left: var(--sidebar-w); min-height: 100vh; }
        .topbar {
            background: #fff; border-bottom: 1px solid #e9ecef;
            padding: .75rem 1.5rem; display: flex;
            align-items: center; justify-content: space-between;
        }
        .content-area { padding: 1.5rem; }
        /* Status badge colours */
        .badge-Applied    { background: #e7f3ff; color: #0d6efd; }
        .badge-Screening  { background: #fff3cd; color: #856404; }
        .badge-Interview  { background: #e8d5ff; color: #6f42c1; }
        .badge-Offer      { background: #d1e7dd; color: #0a5c36; }
        .badge-Rejected   { background: #f8d7da; color: #842029; }
        .badge-Withdrawn  { background: #e9ecef; color: #495057; }
        /* Internship status colours */
        .badge-Shortlisted{ background: #d0ebff; color: #1864ab; }
        .badge-Test       { background: #fff0b3; color: #7d5c00; }
        .badge-Selected   { background: #d3f9d8; color: #2b8a3e; }
        .badge-Completed  { background: #c5f6fa; color: #0c7b9e; }
        /* PPO badge */
        .badge-ppo        { background: #fff3bf; color: #e67700; }
        @@media (max-width: 768px) {
            .sidebar { transform: translateX(-100%); }
            .main-wrap { margin-left: 0; }
        }
    </style>
</head>
<body>
@if (User.Identity?.IsAuthenticated == true)
{
<div class="sidebar">
    <div class="brand">
        <i class="bi bi-mortarboard-fill text-warning me-2 fs-5"></i>
        <span>PlacementHub</span>
    </div>
    <nav class="nav flex-column mt-2 flex-grow-1">

    @if (User.IsInRole("Student"))
    {
        <span class="section-label">Student</span>
        <a class="nav-link @(ViewContext.RouteData.Values["controller"]?.ToString()=="Dashboard"?"active":"")"
           asp-controller="Dashboard" asp-action="Index">
            <i class="bi bi-speedometer2 me-2"></i>Dashboard
        </a>

        <span class="section-label mt-2">Placement</span>
        <a class="nav-link @(ViewContext.RouteData.Values["controller"]?.ToString()=="Applications"?"active":"")"
           asp-controller="Applications" asp-action="Index">
            <i class="bi bi-briefcase me-2"></i>Job Applications
        </a>
        <a class="nav-link" asp-controller="Applications" asp-action="Create">
            <i class="bi bi-plus-circle me-2"></i>Add Placement
        </a>

        <span class="section-label mt-2">Internship</span>
        <a class="nav-link @(ViewContext.RouteData.Values["controller"]?.ToString()=="Internships"?"active":"")"
           asp-controller="Internships" asp-action="Index">
            <i class="bi bi-building me-2"></i>My Internships
        </a>
        <a class="nav-link" asp-controller="Internships" asp-action="Create">
            <i class="bi bi-plus-circle me-2"></i>Add Internship
        </a>
    }

    @if (User.IsInRole("Faculty"))
    {
        <span class="section-label">Faculty</span>
        <a class="nav-link @(ViewContext.RouteData.Values["controller"]?.ToString()=="Faculty"?"active":"")"
           asp-controller="Faculty" asp-action="Index">
            <i class="bi bi-people me-2"></i>My Students
        </a>
    }

    @if (User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
    {
        <span class="section-label">Admin</span>
        <a class="nav-link @(ViewContext.RouteData.Values["action"]?.ToString()=="Index" && ViewContext.RouteData.Values["controller"]?.ToString()=="Admin"?"active":"")"
           asp-controller="Admin" asp-action="Index">
            <i class="bi bi-graph-up me-2"></i>Analytics
        </a>
        <a class="nav-link" asp-controller="Admin" asp-action="Applications">
            <i class="bi bi-briefcase me-2"></i>All Placements
        </a>
        <a class="nav-link" asp-controller="Admin" asp-action="Internships">
            <i class="bi bi-building me-2"></i>All Internships
        </a>
        <a class="nav-link" asp-controller="Admin" asp-action="Users">
            <i class="bi bi-person-gear me-2"></i>Users
        </a>
        <a class="nav-link" asp-controller="Admin" asp-action="Companies">
            <i class="bi bi-buildings me-2"></i>Companies
        </a>
    }

    @if (User.IsInRole("SuperAdmin"))
    {
        <a class="nav-link" asp-controller="SuperAdmin" asp-action="Index">
            <i class="bi bi-shield-check me-2"></i>Super Admin
        </a>
    }
    </nav>

    <div class="p-3 border-top border-secondary">
        <form asp-controller="Account" asp-action="Logout" method="post">
            @Html.AntiForgeryToken()
            <button type="submit" class="btn btn-sm btn-outline-light w-100">
                <i class="bi bi-box-arrow-left me-1"></i>Logout
            </button>
        </form>
    </div>
</div>
}

<div class="@(User.Identity?.IsAuthenticated == true ? "main-wrap" : "")">
@if (User.Identity?.IsAuthenticated == true)
{
<div class="topbar">
    <span class="fw-semibold text-muted small">@ViewData["Title"]</span>
    <div class="d-flex align-items-center gap-2">
        <span class="badge bg-primary">@User.Identity?.Name</span>
    </div>
</div>
}
<div class="@(User.Identity?.IsAuthenticated == true ? "content-area" : "")">
    @if (TempData["Success"] != null)
    {
    <div class="alert alert-success alert-dismissible fade show" role="alert">
        <i class="bi bi-check-circle me-2"></i>@TempData["Success"]
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>
    }
    @RenderBody()
</div>
</div>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.2/dist/chart.umd.min.js"></script>
@await RenderSectionAsync("Scripts", required: false)
</body>
</html>
```

---

### FILE: `Views/Account/Login.cshtml`
```html
@model PlacementTracker.ViewModels.LoginViewModel
@{ Layout = null; ViewData["Title"] = "Login"; }
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8"/>
    <title>PlacementHub — Login</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css"/>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css"/>
    <style>
        body { min-height:100vh; background:linear-gradient(135deg,#1a1f36,#2d3561); display:flex; align-items:center; justify-content:center; }
        .login-card { background:#fff; border-radius:16px; padding:2.5rem; width:100%; max-width:420px; box-shadow:0 20px 60px rgba(0,0,0,.3); }
    </style>
</head>
<body>
<div class="login-card">
    <div class="text-center mb-4">
        <i class="bi bi-mortarboard-fill text-primary" style="font-size:2.5rem;"></i>
        <h4 class="fw-bold mt-2">PlacementHub</h4>
        <p class="text-muted small">Placement & Internship Tracker</p>
    </div>
    <form asp-action="Login" method="post">
        @Html.AntiForgeryToken()
        <div asp-validation-summary="All" class="text-danger small mb-2"></div>
        <div class="mb-3">
            <label class="form-label fw-semibold">Email</label>
            <input asp-for="Email" class="form-control" placeholder="you@college.edu" autofocus/>
        </div>
        <div class="mb-3">
            <label class="form-label fw-semibold">Password</label>
            <input asp-for="Password" type="password" class="form-control"/>
        </div>
        <div class="mb-3 form-check">
            <input asp-for="RememberMe" class="form-check-input" type="checkbox"/>
            <label class="form-check-label small">Remember me</label>
        </div>
        <button type="submit" class="btn btn-primary w-100">Sign In</button>
    </form>
    <hr/>
    <p class="text-center text-muted small mb-0">
        Demo: rahul@student.com / Student@123
    </p>
</div>
</body>
</html>
```

---

### FILE: `Views/Dashboard/Index.cshtml`
```html
@model PlacementTracker.ViewModels.DashboardViewModel
@{ ViewData["Title"] = "My Dashboard"; }

<div class="d-flex justify-content-between align-items-center mb-3">
    <h5 class="fw-bold mb-0">Welcome back 👋</h5>
</div>

<!-- ═══ PLACEMENT SUMMARY CARDS ═══ -->
<p class="text-muted small fw-semibold text-uppercase mb-1" style="letter-spacing:1px;">
    <i class="bi bi-briefcase me-1"></i> Placement Pipeline
</p>
<div class="row g-3 mb-4">
@foreach (var (label, val, color, icon) in new[] {
    ("Total Applied",  (object)Model.TotalApplications, "primary",   "collection"),
    ("Screening",      (object)Model.Screening,          "warning",   "funnel"),
    ("Interview",      (object)Model.Interview,           "purple",    "camera-video"),
    ("Offers",         (object)Model.Offer,               "success",   "trophy"),
    ("Rejected",       (object)Model.Rejected,            "danger",    "x-circle"),
}) {
<div class="col-6 col-sm-4 col-md-2">
    <div class="card border-0 shadow-sm text-center p-3 h-100">
        <i class="bi bi-@icon fs-4 text-@color"></i>
        <div class="fs-3 fw-bold text-@color mt-1">@val</div>
        <small class="text-muted">@label</small>
    </div>
</div>
}
</div>

<!-- ═══ INTERNSHIP SUMMARY CARDS ═══ -->
<p class="text-muted small fw-semibold text-uppercase mb-1" style="letter-spacing:1px;">
    <i class="bi bi-building me-1"></i> Internship Pipeline
</p>
<div class="row g-3 mb-4">
@foreach (var (label, val, color, icon) in new[] {
    ("Total",          (object)Model.TotalInternships,      "info",    "building"),
    ("Shortlisted",    (object)Model.InternshipShortlisted,  "primary", "funnel"),
    ("Selected",       (object)Model.InternshipSelected,     "success", "check-circle"),
    ("Completed",      (object)Model.InternshipCompleted,    "teal",    "patch-check"),
    ("PPO Converted",  (object)Model.PPOConverted,           "warning", "star"),
}) {
<div class="col-6 col-sm-4 col-md-2">
    <div class="card border-0 shadow-sm text-center p-3 h-100">
        <i class="bi bi-@icon fs-4 text-@color"></i>
        <div class="fs-3 fw-bold text-@color mt-1">@val</div>
        <small class="text-muted">@label</small>
    </div>
</div>
}
</div>

@if (Model.HasOngoingInternship)
{
<div class="alert alert-success d-flex align-items-center mb-3">
    <i class="bi bi-building-check fs-5 me-2"></i>
    <span><strong>You have an ongoing internship!</strong> Keep logging your progress.</span>
</div>
}

<!-- ═══ TWO COLUMNS: Upcoming Interviews ═══ -->
<div class="row g-4">
    <!-- Placement Interviews -->
    <div class="col-md-6">
        <div class="card border-0 shadow-sm">
            <div class="card-header bg-white fw-semibold border-0 pt-3">
                <i class="bi bi-camera-video text-primary me-2"></i>Upcoming Placement Interviews
            </div>
            <div class="card-body p-0">
            @if (!Model.UpcomingInterviews.Any())
            {
                <p class="text-muted small p-3 mb-0">No upcoming placement interviews.</p>
            }
            else
            {
                <ul class="list-group list-group-flush">
                @foreach (var iv in Model.UpcomingInterviews)
                {
                <li class="list-group-item d-flex justify-content-between align-items-start">
                    <div>
                        <div class="fw-semibold small">@iv.InterviewType Round</div>
                        <small class="text-muted">
                            @iv.InterviewDate.ToString("dd MMM, ddd")
                            @(iv.InterviewTime.HasValue ? "@ " + DateTime.Today.Add(iv.InterviewTime.Value).ToString("hh:mm tt") : "")
                        </small>
                    </div>
                    <span class="badge rounded-pill @(iv.Outcome == "Cleared" ? "bg-success" : "bg-warning text-dark")">
                        @iv.Outcome
                    </span>
                </li>
                }
                </ul>
            }
            </div>
        </div>
    </div>

    <!-- Internship Interviews -->
    <div class="col-md-6">
        <div class="card border-0 shadow-sm">
            <div class="card-header bg-white fw-semibold border-0 pt-3">
                <i class="bi bi-building text-info me-2"></i>Upcoming Internship Interviews
            </div>
            <div class="card-body p-0">
            @if (!Model.UpcomingInternshipInterviews.Any())
            {
                <p class="text-muted small p-3 mb-0">No upcoming internship interviews.</p>
            }
            else
            {
                <ul class="list-group list-group-flush">
                @foreach (var iv in Model.UpcomingInternshipInterviews)
                {
                <li class="list-group-item d-flex justify-content-between align-items-start">
                    <div>
                        <div class="fw-semibold small">@iv.RoundType Round</div>
                        <small class="text-muted">
                            @iv.InterviewDate.ToString("dd MMM, ddd")
                            @(iv.InterviewTime.HasValue ? "@ " + DateTime.Today.Add(iv.InterviewTime.Value).ToString("hh:mm tt") : "")
                        </small>
                    </div>
                    <span class="badge rounded-pill @(iv.Outcome == "Cleared" ? "bg-success" : "bg-warning text-dark")">
                        @iv.Outcome
                    </span>
                </li>
                }
                </ul>
            }
            </div>
        </div>
    </div>
</div>

<!-- ═══ Recent Activity ═══ -->
<div class="row g-4 mt-1">
    <div class="col-md-6">
        <div class="card border-0 shadow-sm">
            <div class="card-header bg-white fw-semibold border-0 pt-3">
                <i class="bi bi-briefcase text-primary me-2"></i>Recent Placement Applications
                <a asp-controller="Applications" asp-action="Index" class="btn btn-sm btn-outline-primary float-end">View All</a>
            </div>
            <div class="table-responsive">
            <table class="table table-sm table-hover mb-0">
                <tbody>
                @foreach (var a in Model.RecentApplications)
                {
                <tr>
                    <td>@a.CompanyName <small class="text-muted">— @a.Role</small></td>
                    <td><span class="badge badge-@a.Status">@a.Status</span></td>
                </tr>
                }
                </tbody>
            </table>
            </div>
        </div>
    </div>
    <div class="col-md-6">
        <div class="card border-0 shadow-sm">
            <div class="card-header bg-white fw-semibold border-0 pt-3">
                <i class="bi bi-building text-info me-2"></i>Recent Internship Applications
                <a asp-controller="Internships" asp-action="Index" class="btn btn-sm btn-outline-info float-end">View All</a>
            </div>
            <div class="table-responsive">
            <table class="table table-sm table-hover mb-0">
                <tbody>
                @foreach (var a in Model.RecentInternships)
                {
                <tr>
                    <td>
                        @a.CompanyName <small class="text-muted">— @a.Role</small>
                        @if (a.IsPPOConverted) { <span class="badge badge-ppo ms-1">PPO</span> }
                    </td>
                    <td><span class="badge badge-@a.Status">@a.Status</span></td>
                </tr>
                }
                </tbody>
            </table>
            </div>
        </div>
    </div>
</div>
```

---

### FILE: `Views/Internships/Index.cshtml`
```html
@model List<PlacementTracker.Models.InternshipApplication>
@{ ViewData["Title"] = "My Internships"; }
@using PlacementTracker.ViewModels

<div class="d-flex justify-content-between align-items-center mb-3">
    <h5 class="fw-bold mb-0"><i class="bi bi-building me-2"></i>My Internship Applications</h5>
    <div class="d-flex gap-2">
        <a asp-action="Create" class="btn btn-primary btn-sm">
            <i class="bi bi-plus-lg me-1"></i>Add Internship
        </a>
        <a asp-action="ExportExcel" class="btn btn-outline-success btn-sm">
            <i class="bi bi-file-earmark-excel me-1"></i>Export
        </a>
    </div>
</div>

<!-- Summary Cards -->
<div class="row g-2 mb-3">
@{
    var total      = Model.Count;
    var selected   = Model.Count(a => a.Status == "Selected");
    var completed  = Model.Count(a => a.Status == "Completed");
    var ppoCount   = Model.Count(a => a.IsPPOConverted);
    var ongoing    = Model.Count(a => a.IsOngoing);
}
@foreach (var (label, val, color) in new[] {
    ("Total", (object)total, "primary"),
    ("Selected", (object)selected, "success"),
    ("Completed", (object)completed, "info"),
    ("PPO", (object)ppoCount, "warning"),
    ("Ongoing", (object)ongoing, "teal"),
}) {
<div class="col-6 col-md-2">
    <div class="card border-0 shadow-sm text-center py-2 h-100">
        <div class="fs-4 fw-bold text-@color">@val</div>
        <small class="text-muted">@label</small>
    </div>
</div>
}
</div>

<!-- Filters -->
<div class="card border-0 shadow-sm mb-3">
<div class="card-body py-2">
<form method="get" class="row g-2 align-items-end">
    <div class="col-auto">
        <select name="status" class="form-select form-select-sm">
            <option value="">All Status</option>
            @foreach (var s in InternshipFormViewModel.StatusList)
            { <option value="@s" @(ViewBag.Filter_Status == s ? "selected" : "")>@s</option> }
        </select>
    </div>
    <div class="col-auto">
        <select name="internshipType" class="form-select form-select-sm">
            <option value="">All Types</option>
            @foreach (var t in InternshipFormViewModel.TypeList)
            { <option value="@t" @(ViewBag.Filter_InternshipType == t ? "selected" : "")>@t</option> }
        </select>
    </div>
    <div class="col-auto">
        <select name="workMode" class="form-select form-select-sm">
            <option value="">All Modes</option>
            @foreach (var m in InternshipFormViewModel.ModeList)
            { <option value="@m" @(ViewBag.Filter_WorkMode == m ? "selected" : "")>@m</option> }
        </select>
    </div>
    <div class="col-auto">
        <input name="company" type="text" class="form-control form-control-sm"
               placeholder="Company..." value="@ViewBag.Filter_Company"/>
    </div>
    <div class="col-auto">
        <input name="from" type="date" class="form-control form-control-sm" value="@ViewBag.Filter_From"/>
    </div>
    <div class="col-auto">
        <input name="to" type="date" class="form-control form-control-sm" value="@ViewBag.Filter_To"/>
    </div>
    <div class="col-auto d-flex gap-1">
        <button type="submit" class="btn btn-primary btn-sm">Filter</button>
        <a asp-action="Index" class="btn btn-outline-secondary btn-sm">Clear</a>
    </div>
</form>
</div>
</div>

<!-- Table -->
@if (!Model.Any())
{
<div class="text-center py-5">
    <i class="bi bi-building fs-1 text-muted opacity-50"></i>
    <p class="mt-2 text-muted">No internship applications yet. <a asp-action="Create">Add one!</a></p>
</div>
}
else
{
<div class="card border-0 shadow-sm">
<div class="table-responsive">
<table class="table table-hover table-sm mb-0">
    <thead class="table-light">
    <tr>
        <th>Company</th>
        <th>Role</th>
        <th>Type</th>
        <th>Mode</th>
        <th>Status</th>
        <th>Duration</th>
        <th>Stipend</th>
        <th>PPO</th>
        <th>Actions</th>
    </tr>
    </thead>
    <tbody>
    @foreach (var a in Model)
    {
    <tr>
        <td class="fw-semibold">
            @a.CompanyName
            @if (a.IsOngoing) {
                <span class="badge bg-success-subtle text-success ms-1">Ongoing</span>
            }
        </td>
        <td>@a.Role</td>
        <td><span class="badge bg-light text-dark border">@a.InternshipType</span></td>
        <td>
            <span class="badge @(a.WorkMode=="Remote"?"bg-info text-dark":a.WorkMode=="Hybrid"?"bg-warning text-dark":"bg-secondary")">
                @a.WorkMode
            </span>
        </td>
        <td><span class="badge badge-@a.Status">@a.Status</span></td>
        <td class="small text-muted">
            @if (a.DurationDays.HasValue) { <span>@a.DurationDays days</span> }
            else { <span>—</span> }
        </td>
        <td class="small">@(a.Stipend != null ? $"₹{a.Stipend}/mo" : "—")</td>
        <td>
            @if (a.IsPPOConverted) {
                <span class="badge badge-ppo"><i class="bi bi-star-fill"></i> PPO</span>
            } else { <span class="text-muted">—</span> }
        </td>
        <td>
            <div class="d-flex gap-1">
                <a asp-action="Details" asp-route-id="@a.Id"
                   class="btn btn-xs btn-outline-primary btn-sm">
                    <i class="bi bi-eye"></i>
                </a>
                <a asp-action="Edit" asp-route-id="@a.Id"
                   class="btn btn-xs btn-outline-secondary btn-sm">
                    <i class="bi bi-pencil"></i>
                </a>
                <!-- Quick status update -->
                <div class="dropdown">
                    <button class="btn btn-sm btn-outline-dark dropdown-toggle"
                            data-bs-toggle="dropdown">Status</button>
                    <ul class="dropdown-menu dropdown-menu-end shadow-sm">
                    @foreach (var s in InternshipFormViewModel.StatusList)
                    {
                    <li>
                        <form asp-action="UpdateStatus" method="post">
                            @Html.AntiForgeryToken()
                            <input type="hidden" name="id" value="@a.Id"/>
                            <input type="hidden" name="status" value="@s"/>
                            <button type="submit" class="dropdown-item small @(a.Status==s?"fw-bold":"")">
                                @s
                            </button>
                        </form>
                    </li>
                    }
                    </ul>
                </div>
            </div>
        </td>
    </tr>
    }
    </tbody>
</table>
</div>
</div>
}
```

---

### FILE: `Views/Internships/Create.cshtml`
```html
@model PlacementTracker.ViewModels.InternshipFormViewModel
@{ ViewData["Title"] = "Add Internship Application"; }

<div class="row justify-content-center">
<div class="col-md-8">
<div class="card border-0 shadow-sm">
<div class="card-header bg-white border-0 pt-3">
    <h5 class="fw-bold mb-0"><i class="bi bi-building-add me-2 text-info"></i>Add Internship Application</h5>
</div>
<div class="card-body">
<form asp-action="Create" method="post">
    @Html.AntiForgeryToken()
    <div asp-validation-summary="ModelOnly" class="text-danger mb-2"></div>

    <div class="row g-3">
        <div class="col-md-6">
            <label asp-for="CompanyName" class="form-label fw-semibold"></label>
            <input asp-for="CompanyName" class="form-control" placeholder="e.g. Google, TCS..."/>
            <span asp-validation-for="CompanyName" class="text-danger small"></span>
        </div>
        <div class="col-md-6">
            <label asp-for="Role" class="form-label fw-semibold"></label>
            <input asp-for="Role" class="form-control" placeholder="e.g. SDE Intern, Research Trainee..."/>
            <span asp-validation-for="Role" class="text-danger small"></span>
        </div>
        <div class="col-md-4">
            <label asp-for="InternshipType" class="form-label fw-semibold"></label>
            <select asp-for="InternshipType" class="form-select">
                @foreach (var t in ViewBag.TypeList as List<string> ?? new())
                { <option value="@t">@t</option> }
            </select>
        </div>
        <div class="col-md-4">
            <label asp-for="WorkMode" class="form-label fw-semibold"></label>
            <select asp-for="WorkMode" class="form-select">
                @foreach (var m in ViewBag.ModeList as List<string> ?? new())
                { <option value="@m">@m</option> }
            </select>
        </div>
        <div class="col-md-4">
            <label asp-for="Status" class="form-label fw-semibold"></label>
            <select asp-for="Status" class="form-select">
                @foreach (var s in ViewBag.StatusList as List<string> ?? new())
                { <option value="@s">@s</option> }
            </select>
        </div>
        <div class="col-md-4">
            <label asp-for="AppliedDate" class="form-label fw-semibold"></label>
            <input asp-for="AppliedDate" type="date" class="form-control"/>
        </div>
        <div class="col-md-4">
            <label asp-for="StartDate" class="form-label fw-semibold"></label>
            <input asp-for="StartDate" type="date" class="form-control"/>
        </div>
        <div class="col-md-4">
            <label asp-for="EndDate" class="form-label fw-semibold"></label>
            <input asp-for="EndDate" type="date" class="form-control"/>
        </div>
        <div class="col-md-4">
            <label asp-for="Stipend" class="form-label fw-semibold"></label>
            <div class="input-group">
                <span class="input-group-text">₹</span>
                <input asp-for="Stipend" class="form-control" placeholder="15000"/>
                <span class="input-group-text">/month</span>
            </div>
        </div>
        <div class="col-md-4">
            <label asp-for="Location" class="form-label fw-semibold"></label>
            <input asp-for="Location" class="form-control" placeholder="City or Remote"/>
        </div>
        <div class="col-md-4">
            <label asp-for="JobLink" class="form-label fw-semibold"></label>
            <input asp-for="JobLink" type="url" class="form-control" placeholder="https://..."/>
        </div>
        <div class="col-12">
            <label asp-for="Notes" class="form-label fw-semibold"></label>
            <textarea asp-for="Notes" class="form-control" rows="2"
                      placeholder="Anything to remember about this application..."></textarea>
        </div>
        <div class="col-md-4">
            <div class="form-check form-switch mt-3">
                <input asp-for="IsPPOConverted" class="form-check-input" type="checkbox" id="ppoSwitch"/>
                <label class="form-check-label fw-semibold" for="ppoSwitch">PPO Converted</label>
            </div>
        </div>
        <div class="col-md-4" id="ppoPackageDiv" style="display:none;">
            <label asp-for="PPOPackage" class="form-label fw-semibold"></label>
            <input asp-for="PPOPackage" class="form-control" placeholder="e.g. 8 LPA"/>
        </div>
        <div class="col-md-4">
            <div class="form-check form-switch mt-3">
                <input asp-for="CertificateReceived" class="form-check-input" type="checkbox"/>
                <label class="form-check-label fw-semibold">Certificate Received</label>
            </div>
        </div>
    </div>

    <div class="mt-4 d-flex gap-2">
        <button type="submit" class="btn btn-primary">
            <i class="bi bi-building-add me-1"></i>Save Internship
        </button>
        <a asp-action="Index" class="btn btn-outline-secondary">Cancel</a>
    </div>
</form>
</div>
</div>
</div>
</div>

@section Scripts {
<script>
const ppoSwitch = document.getElementById('ppoSwitch');
const ppoDiv    = document.getElementById('ppoPackageDiv');
ppoSwitch.addEventListener('change', () => {
    ppoDiv.style.display = ppoSwitch.checked ? 'block' : 'none';
});
if (ppoSwitch.checked) ppoDiv.style.display = 'block';
</script>
}
```

---

### FILE: `Views/Internships/Edit.cshtml`
```html
@model PlacementTracker.ViewModels.InternshipFormViewModel
@{ ViewData["Title"] = "Edit Internship Application"; }

<div class="row justify-content-center">
<div class="col-md-8">
<div class="card border-0 shadow-sm">
<div class="card-header bg-white border-0 pt-3">
    <h5 class="fw-bold mb-0"><i class="bi bi-pencil-square me-2 text-warning"></i>Edit Internship Application</h5>
</div>
<div class="card-body">
<form asp-action="Edit" asp-route-id="@Model.Id" method="post">
    @Html.AntiForgeryToken()
    <div asp-validation-summary="ModelOnly" class="text-danger mb-2"></div>

    <div class="row g-3">
        <div class="col-md-6">
            <label asp-for="CompanyName" class="form-label fw-semibold"></label>
            <input asp-for="CompanyName" class="form-control"/>
            <span asp-validation-for="CompanyName" class="text-danger small"></span>
        </div>
        <div class="col-md-6">
            <label asp-for="Role" class="form-label fw-semibold"></label>
            <input asp-for="Role" class="form-control"/>
        </div>
        <div class="col-md-4">
            <label asp-for="InternshipType" class="form-label fw-semibold"></label>
            <select asp-for="InternshipType" class="form-select">
                @foreach (var t in ViewBag.TypeList as List<string> ?? new())
                { <option value="@t">@t</option> }
            </select>
        </div>
        <div class="col-md-4">
            <label asp-for="WorkMode" class="form-label fw-semibold"></label>
            <select asp-for="WorkMode" class="form-select">
                @foreach (var m in ViewBag.ModeList as List<string> ?? new())
                { <option value="@m">@m</option> }
            </select>
        </div>
        <div class="col-md-4">
            <label asp-for="Status" class="form-label fw-semibold"></label>
            <select asp-for="Status" class="form-select">
                @foreach (var s in ViewBag.StatusList as List<string> ?? new())
                { <option value="@s">@s</option> }
            </select>
        </div>
        <div class="col-md-4">
            <label asp-for="AppliedDate" class="form-label fw-semibold"></label>
            <input asp-for="AppliedDate" type="date" class="form-control"/>
        </div>
        <div class="col-md-4">
            <label asp-for="StartDate" class="form-label fw-semibold"></label>
            <input asp-for="StartDate" type="date" class="form-control"/>
        </div>
        <div class="col-md-4">
            <label asp-for="EndDate" class="form-label fw-semibold"></label>
            <input asp-for="EndDate" type="date" class="form-control"/>
        </div>
        <div class="col-md-4">
            <label asp-for="Stipend" class="form-label fw-semibold"></label>
            <div class="input-group">
                <span class="input-group-text">₹</span>
                <input asp-for="Stipend" class="form-control"/>
                <span class="input-group-text">/month</span>
            </div>
        </div>
        <div class="col-md-4">
            <label asp-for="Location" class="form-label fw-semibold"></label>
            <input asp-for="Location" class="form-control"/>
        </div>
        <div class="col-md-4">
            <label asp-for="JobLink" class="form-label fw-semibold"></label>
            <input asp-for="JobLink" type="url" class="form-control"/>
        </div>
        <div class="col-12">
            <label asp-for="Notes" class="form-label fw-semibold"></label>
            <textarea asp-for="Notes" class="form-control" rows="2"></textarea>
        </div>
        <div class="col-md-4">
            <div class="form-check form-switch mt-3">
                <input asp-for="IsPPOConverted" class="form-check-input" type="checkbox" id="ppoSwitch"/>
                <label class="form-check-label fw-semibold" for="ppoSwitch">PPO Converted</label>
            </div>
        </div>
        <div class="col-md-4" id="ppoPackageDiv">
            <label asp-for="PPOPackage" class="form-label fw-semibold"></label>
            <input asp-for="PPOPackage" class="form-control" placeholder="e.g. 8 LPA"/>
        </div>
        <div class="col-md-4">
            <div class="form-check form-switch mt-3">
                <input asp-for="CertificateReceived" class="form-check-input" type="checkbox"/>
                <label class="form-check-label fw-semibold">Certificate Received</label>
            </div>
        </div>
    </div>

    <div class="mt-4 d-flex gap-2">
        <button type="submit" class="btn btn-warning">
            <i class="bi bi-save me-1"></i>Update
        </button>
        <a asp-action="Details" asp-route-id="@Model.Id" class="btn btn-outline-secondary">Cancel</a>
    </div>
</form>
</div>
</div>
</div>
</div>

@section Scripts {
<script>
const ppoSwitch = document.getElementById('ppoSwitch');
const ppoDiv    = document.getElementById('ppoPackageDiv');
function togglePPO() { ppoDiv.style.display = ppoSwitch.checked ? 'block' : 'none'; }
ppoSwitch.addEventListener('change', togglePPO);
togglePPO();
</script>
}
```

---

### FILE: `Views/Internships/Details.cshtml`
```html
@model PlacementTracker.Models.InternshipApplication
@{ ViewData["Title"] = $"Internship — {Model.CompanyName}"; }
@using PlacementTracker.ViewModels

<div class="d-flex justify-content-between align-items-center mb-3">
    <div>
        <h5 class="fw-bold mb-0">
            @Model.CompanyName
            @if (Model.IsOngoing) { <span class="badge bg-success ms-2">Ongoing</span> }
            @if (Model.IsPPOConverted) { <span class="badge badge-ppo ms-1"><i class="bi bi-star-fill"></i> PPO</span> }
        </h5>
        <small class="text-muted">@Model.Role · @Model.InternshipType · @Model.WorkMode</small>
    </div>
    <div class="d-flex gap-2">
        <a asp-action="Edit" asp-route-id="@Model.Id" class="btn btn-sm btn-outline-warning">
            <i class="bi bi-pencil me-1"></i>Edit
        </a>
        <form asp-action="Delete" method="post"
              onsubmit="return confirm('Remove this internship application?')">
            @Html.AntiForgeryToken()
            <input type="hidden" name="id" value="@Model.Id"/>
            <button class="btn btn-sm btn-outline-danger">
                <i class="bi bi-trash me-1"></i>Delete
            </button>
        </form>
    </div>
</div>

<div class="row g-4">
    <!-- Details Card -->
    <div class="col-md-5">
        <div class="card border-0 shadow-sm h-100">
        <div class="card-body">
            <h6 class="fw-semibold text-muted mb-3">Application Details</h6>
            <dl class="row small">
                <dt class="col-5">Status</dt>
                <dd class="col-7"><span class="badge badge-@Model.Status">@Model.Status</span></dd>
                <dt class="col-5">Type</dt>
                <dd class="col-7">@Model.InternshipType</dd>
                <dt class="col-5">Work Mode</dt>
                <dd class="col-7">@Model.WorkMode</dd>
                <dt class="col-5">Applied On</dt>
                <dd class="col-7">@Model.AppliedDate.ToString("dd MMM yyyy")</dd>
                <dt class="col-5">Start Date</dt>
                <dd class="col-7">@(Model.StartDate?.ToString("dd MMM yyyy") ?? "—")</dd>
                <dt class="col-5">End Date</dt>
                <dd class="col-7">@(Model.EndDate?.ToString("dd MMM yyyy") ?? "—")</dd>
                <dt class="col-5">Duration</dt>
                <dd class="col-7">@(Model.DurationDays.HasValue ? $"{Model.DurationDays} days" : "—")</dd>
                <dt class="col-5">Stipend</dt>
                <dd class="col-7">@(Model.Stipend != null ? $"₹{Model.Stipend}/month" : "—")</dd>
                <dt class="col-5">Location</dt>
                <dd class="col-7">@(Model.Location ?? "—")</dd>
                <dt class="col-5">PPO Converted</dt>
                <dd class="col-7">
                    @if (Model.IsPPOConverted) {
                        <span class="badge badge-ppo">Yes — @Model.PPOPackage</span>
                    } else { <span class="text-muted">No</span> }
                </dd>
                <dt class="col-5">Certificate</dt>
                <dd class="col-7">
                    @if (Model.CertificateReceived) {
                        <span class="badge bg-success">Received</span>
                    } else { <span class="text-muted">Pending</span> }
                </dd>
                @if (!string.IsNullOrEmpty(Model.JobLink))
                {
                <dt class="col-5">Link</dt>
                <dd class="col-7"><a href="@Model.JobLink" target="_blank" rel="noopener">Open ↗</a></dd>
                }
            </dl>
            @if (!string.IsNullOrEmpty(Model.Notes))
            {
            <div class="bg-light rounded p-2 mt-2 small">
                <i class="bi bi-sticky me-1 text-muted"></i>@Model.Notes
            </div>
            }

            <!-- Quick Actions -->
            <div class="mt-3 d-flex gap-2 flex-wrap">
                @if (!Model.IsPPOConverted)
                {
                <button class="btn btn-sm btn-outline-warning" data-bs-toggle="modal" data-bs-target="#ppoModal">
                    <i class="bi bi-star me-1"></i>Mark PPO
                </button>
                }
                @if (!Model.CertificateReceived)
                {
                <form asp-action="MarkCertificate" method="post">
                    @Html.AntiForgeryToken()
                    <input type="hidden" name="id" value="@Model.Id"/>
                    <button type="submit" class="btn btn-sm btn-outline-success">
                        <i class="bi bi-patch-check me-1"></i>Mark Certificate
                    </button>
                </form>
                }
            </div>
        </div>
        </div>
    </div>

    <!-- Interviews Card -->
    <div class="col-md-7">
        <div class="card border-0 shadow-sm mb-3">
        <div class="card-header bg-white border-0 d-flex justify-content-between align-items-center pt-3">
            <h6 class="fw-semibold mb-0"><i class="bi bi-camera-video text-info me-2"></i>Interview Rounds</h6>
            <button class="btn btn-sm btn-outline-info" data-bs-toggle="modal" data-bs-target="#addInterviewModal">
                <i class="bi bi-plus me-1"></i>Add Round
            </button>
        </div>
        <div class="card-body p-0">
        @if (!Model.Interviews.Any())
        {
            <p class="text-muted small p-3 mb-0">No interview rounds scheduled yet.</p>
        }
        else
        {
            <ul class="list-group list-group-flush">
            @foreach (var iv in Model.Interviews.OrderBy(i => i.InterviewDate))
            {
            <li class="list-group-item">
                <div class="d-flex justify-content-between align-items-start">
                    <div>
                        <span class="badge bg-secondary me-1">@iv.RoundType</span>
                        <strong>@iv.InterviewDate.ToString("dd MMM yyyy")</strong>
                        @if (iv.InterviewTime.HasValue) {
                            <small class="text-muted ms-1">
                                @ @DateTime.Today.Add(iv.InterviewTime.Value).ToString("hh:mm tt")
                            </small>
                        }
                        @if (!string.IsNullOrEmpty(iv.Venue)) {
                            <small class="text-muted ms-2"><i class="bi bi-geo-alt"></i> @iv.Venue</small>
                        }
                        @if (!string.IsNullOrEmpty(iv.MeetingLink)) {
                            <a href="@iv.MeetingLink" class="btn btn-xs btn-link p-0 ms-2" target="_blank">Join ↗</a>
                        }
                        @if (!string.IsNullOrEmpty(iv.Notes)) {
                            <small class="d-block text-muted">@iv.Notes</small>
                        }
                    </div>
                    <div class="d-flex gap-1 align-items-center">
                        <!-- Outcome selector -->
                        <form asp-action="UpdateInterviewOutcome" method="post" class="d-inline">
                            @Html.AntiForgeryToken()
                            <input type="hidden" name="interviewId" value="@iv.Id"/>
                            <input type="hidden" name="appId" value="@Model.Id"/>
                            <select name="outcome" class="form-select form-select-sm w-auto"
                                    onchange="this.form.submit()">
                                @foreach (var o in new[]{"Pending","Cleared","Not Cleared"})
                                { <option @(iv.Outcome==o?"selected":"")>@o</option> }
                            </select>
                        </form>
                        <!-- Delete -->
                        <form asp-action="DeleteInterview" method="post">
                            @Html.AntiForgeryToken()
                            <input type="hidden" name="interviewId" value="@iv.Id"/>
                            <input type="hidden" name="appId" value="@Model.Id"/>
                            <button type="submit" class="btn btn-sm btn-outline-danger"
                                    onclick="return confirm('Delete this round?')">
                                <i class="bi bi-trash"></i>
                            </button>
                        </form>
                    </div>
                </div>
            </li>
            }
            </ul>
        }
        </div>
        </div>

        <!-- Faculty Remarks -->
        @if (Model.Remarks.Any())
        {
        <div class="card border-0 shadow-sm">
        <div class="card-header bg-white border-0 pt-3">
            <h6 class="fw-semibold mb-0"><i class="bi bi-chat-quote text-warning me-2"></i>Faculty Remarks</h6>
        </div>
        <div class="card-body p-0">
            <ul class="list-group list-group-flush">
            @foreach (var r in Model.Remarks)
            {
            <li class="list-group-item">
                <div class="fw-semibold small">@r.Faculty?.FullName</div>
                <div class="small">@r.Comment</div>
                <small class="text-muted">@r.CreatedAt.ToString("dd MMM, HH:mm")</small>
            </li>
            }
            </ul>
        </div>
        </div>
        }
    </div>
</div>

<!-- Add Interview Modal -->
<div class="modal fade" id="addInterviewModal" tabindex="-1">
<div class="modal-dialog"><div class="modal-content">
<div class="modal-header">
    <h6 class="modal-title fw-bold">Schedule Interview Round</h6>
    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
</div>
<form asp-action="AddInterview" method="post">
@Html.AntiForgeryToken()
<input type="hidden" name="appId" value="@Model.Id"/>
<div class="modal-body">
    <div class="mb-3">
        <label class="form-label fw-semibold">Round Type *</label>
        <select name="roundType" class="form-select" required>
            @foreach (var t in new[]{"Technical","HR","Aptitude","Group Discussion","Assignment","Final"})
            { <option>@t</option> }
        </select>
    </div>
    <div class="row g-2">
        <div class="col-md-6">
            <label class="form-label fw-semibold">Date *</label>
            <input type="date" name="interviewDate" class="form-control" required/>
        </div>
        <div class="col-md-6">
            <label class="form-label fw-semibold">Time</label>
            <input type="time" name="interviewTime" class="form-control"/>
        </div>
    </div>
    <div class="mb-3 mt-2">
        <label class="form-label fw-semibold">Meeting Link</label>
        <input type="url" name="meetingLink" class="form-control" placeholder="https://..."/>
    </div>
    <div class="mb-3">
        <label class="form-label fw-semibold">Venue</label>
        <input name="venue" class="form-control" placeholder="Room / Online"/>
    </div>
    <div class="mb-3">
        <label class="form-label fw-semibold">Notes</label>
        <textarea name="notes" class="form-control" rows="2"></textarea>
    </div>
</div>
<div class="modal-footer">
    <button type="submit" class="btn btn-info">Schedule Round</button>
    <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancel</button>
</div>
</form>
</div></div>
</div>

<!-- PPO Modal -->
<div class="modal fade" id="ppoModal" tabindex="-1">
<div class="modal-dialog modal-sm"><div class="modal-content">
<div class="modal-header">
    <h6 class="modal-title fw-bold">🎉 Mark as PPO Converted</h6>
    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
</div>
<form asp-action="MarkPPO" method="post">
@Html.AntiForgeryToken()
<input type="hidden" name="id" value="@Model.Id"/>
<div class="modal-body">
    <label class="form-label fw-semibold">PPO Package (Optional)</label>
    <div class="input-group">
        <input type="text" name="ppoPackage" class="form-control" placeholder="e.g. 8 LPA"/>
    </div>
</div>
<div class="modal-footer">
    <button type="submit" class="btn btn-warning">Confirm PPO</button>
    <button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancel</button>
</div>
</form>
</div></div>
</div>
```

---

### FILE: `Views/Admin/Index.cshtml` (Admin Analytics Dashboard — Placement + Internship)
```html
@model PlacementTracker.ViewModels.AdminDashboardViewModel
@{ ViewData["Title"] = "Analytics Dashboard"; }
<h4 class="fw-bold mb-4">Placement & Internship Analytics</h4>

<!-- Placement KPI Cards -->
<p class="text-uppercase text-muted small fw-semibold mb-1" style="letter-spacing:1px;">
    <i class="bi bi-briefcase me-1"></i> Placement Overview
</p>
<div class="row g-3 mb-4">
@foreach (var (label, val, color, icon) in new[] {
    ("Total Students",     (object)Model.TotalStudents,    "primary",  "people"),
    ("Total Faculty",      (object)Model.TotalFaculty,     "secondary","person-badge"),
    ("Total Applications", (object)Model.TotalApplications,"info",     "briefcase"),
    ("Total Offers",       (object)Model.TotalOffers,      "success",  "trophy"),
    ("Placement Rate",     (object)$"{Model.PlacementRate}%","warning", "graph-up")
}) {
<div class="col-6 col-md">
    <div class="card border-0 shadow-sm text-center p-3">
        <i class="bi bi-@icon fs-3 text-@color"></i>
        <div class="fs-2 fw-bold text-@color">@val</div>
        <small class="text-muted">@label</small>
    </div>
</div>
}
</div>

<!-- Internship KPI Cards -->
<p class="text-uppercase text-muted small fw-semibold mb-1" style="letter-spacing:1px;">
    <i class="bi bi-building me-1"></i> Internship Overview
</p>
<div class="row g-3 mb-4">
@foreach (var (label, val, color, icon) in new[] {
    ("Total Internships",  (object)Model.TotalInternships,        "info",    "building"),
    ("Selected",           (object)Model.TotalInternshipSelected, "success", "check-circle"),
    ("Completed",          (object)Model.TotalInternshipCompleted,"teal",    "patch-check"),
    ("PPO Converted",      (object)Model.TotalPPOConverted,       "warning", "star-fill"),
    ("PPO Rate",           (object)$"{Model.InternshipConversionRate}%","orange","percent"),
}) {
<div class="col-6 col-md">
    <div class="card border-0 shadow-sm text-center p-3">
        <i class="bi bi-@icon fs-3 text-@color"></i>
        <div class="fs-2 fw-bold text-@color">@val</div>
        <small class="text-muted">@label</small>
    </div>
</div>
}
</div>

<!-- Charts Row -->
<div class="row g-4 mb-4">
    <div class="col-md-4">
        <div class="card border-0 shadow-sm p-3">
            <h6 class="fw-semibold mb-3">Placement Status</h6>
            <canvas id="statusChart" height="220"></canvas>
        </div>
    </div>
    <div class="col-md-4">
        <div class="card border-0 shadow-sm p-3">
            <h6 class="fw-semibold mb-3">Internship Status</h6>
            <canvas id="internshipStatusChart" height="220"></canvas>
        </div>
    </div>
    <div class="col-md-4">
        <div class="card border-0 shadow-sm p-3">
            <h6 class="fw-semibold mb-3">Internship Types</h6>
            <canvas id="internshipTypeChart" height="220"></canvas>
        </div>
    </div>
</div>

<div class="row g-4 mb-4">
    <div class="col-md-6">
        <div class="card border-0 shadow-sm p-3">
            <h6 class="fw-semibold mb-3">Top Companies — Placements</h6>
            <canvas id="companyChart" height="200"></canvas>
        </div>
    </div>
    <div class="col-md-6">
        <div class="card border-0 shadow-sm p-3">
            <h6 class="fw-semibold mb-3">Top Companies — Internships</h6>
            <canvas id="internCompanyChart" height="200"></canvas>
        </div>
    </div>
</div>

<!-- Quick Links -->
<div class="card border-0 shadow-sm mb-4">
<div class="card-body">
    <h6 class="fw-semibold mb-3">Quick Links</h6>
    <div class="d-flex flex-wrap gap-2">
        <a asp-action="Users"        class="btn btn-outline-primary btn-sm"><i class="bi bi-person-plus me-1"></i>Manage Users</a>
        <a asp-action="Applications" class="btn btn-outline-secondary btn-sm"><i class="bi bi-briefcase me-1"></i>All Placements</a>
        <a asp-action="Internships"  class="btn btn-outline-info btn-sm"><i class="bi bi-building me-1"></i>All Internships</a>
        <a asp-action="Companies"    class="btn btn-outline-success btn-sm"><i class="bi bi-buildings me-1"></i>Companies</a>
        <a asp-action="ExportExcel"  class="btn btn-outline-warning btn-sm"><i class="bi bi-file-earmark-excel me-1"></i>Export Placements</a>
        <a asp-action="ExportInternshipsExcel" class="btn btn-outline-warning btn-sm"><i class="bi bi-file-earmark-excel me-1"></i>Export Internships</a>
    </div>
</div>
</div>

<!-- Recent Activity Tables -->
<div class="row g-4">
<div class="col-md-6">
    <div class="card border-0 shadow-sm">
    <div class="card-body">
        <h6 class="fw-semibold mb-3">Recent Placement Activity</h6>
        <div class="table-responsive">
        <table class="table table-sm table-hover">
            <thead class="table-light"><tr>
                <th>Student</th><th>Company</th><th>Status</th><th>Updated</th>
            </tr></thead>
            <tbody>
            @foreach (var r in Model.RecentActivity)
            {
            <tr>
                <td>@r.StudentName</td>
                <td>@r.CompanyName</td>
                <td><span class="badge badge-@r.Status">@r.Status</span></td>
                <td>@r.UpdatedAt.ToString("dd MMM, HH:mm")</td>
            </tr>
            }
            </tbody>
        </table>
        </div>
    </div>
    </div>
</div>
<div class="col-md-6">
    <div class="card border-0 shadow-sm">
    <div class="card-body">
        <h6 class="fw-semibold mb-3">Recent Internship Activity</h6>
        <div class="table-responsive">
        <table class="table table-sm table-hover">
            <thead class="table-light"><tr>
                <th>Student</th><th>Company</th><th>Type</th><th>Status</th>
            </tr></thead>
            <tbody>
            @foreach (var r in Model.RecentInternshipActivity)
            {
            <tr>
                <td>@r.StudentName</td>
                <td>@r.CompanyName</td>
                <td><span class="badge bg-light text-dark border">@r.InternshipType</span></td>
                <td><span class="badge badge-@r.Status">@r.Status</span></td>
            </tr>
            }
            </tbody>
        </table>
        </div>
    </div>
    </div>
</div>
</div>

@section Scripts {
<script>
const sd  = @Html.Raw(System.Text.Json.JsonSerializer.Serialize(Model.StatusDistribution));
const isd = @Html.Raw(System.Text.Json.JsonSerializer.Serialize(Model.InternshipStatusDistribution));
const itd = @Html.Raw(System.Text.Json.JsonSerializer.Serialize(Model.InternshipTypeDistribution));
const cd  = @Html.Raw(System.Text.Json.JsonSerializer.Serialize(Model.TopCompanies));
const icd = @Html.Raw(System.Text.Json.JsonSerializer.Serialize(Model.TopInternshipCompanies));

const COLORS = ['#0d6efd','#6f42c1','#fd7e14','#198754','#dc3545','#20c997','#0dcaf0'];

function doughnut(id, data) {
    new Chart(document.getElementById(id), {
        type: 'doughnut',
        data: { labels: Object.keys(data), datasets: [{
            data: Object.values(data), borderWidth: 0, backgroundColor: COLORS
        }]},
        options: { responsive:true, plugins:{ legend:{ position:'bottom' } }, cutout:'60%' }
    });
}
function hbar(id, data, color) {
    new Chart(document.getElementById(id), {
        type: 'bar',
        data: { labels: Object.keys(data), datasets: [{
            data: Object.values(data), backgroundColor: color, borderRadius: 6
        }]},
        options: { indexAxis:'y', responsive:true, plugins:{ legend:{ display:false } } }
    });
}

doughnut('statusChart', sd);
doughnut('internshipStatusChart', isd);
doughnut('internshipTypeChart', itd);
hbar('companyChart', cd, '#0d6efd');
hbar('internCompanyChart', icd, '#0dcaf0');
</script>
}
```

---

### FILE: `Views/Admin/Internships.cshtml` (Admin — All Internships)
```html
@model List<PlacementTracker.Models.InternshipApplication>
@{ ViewData["Title"] = "All Internship Applications"; }
@using PlacementTracker.ViewModels

<div class="d-flex justify-content-between align-items-center mb-3">
    <h5 class="fw-bold mb-0"><i class="bi bi-building me-2"></i>All Internship Applications</h5>
    <a asp-action="ExportInternshipsExcel" class="btn btn-outline-success btn-sm">
        <i class="bi bi-file-earmark-excel me-1"></i>Export Excel
    </a>
</div>

<!-- Filters -->
<div class="card border-0 shadow-sm mb-3">
<div class="card-body py-2">
<form method="get" class="row g-2 align-items-end">
    <div class="col-auto">
        <select name="status" class="form-select form-select-sm">
            <option value="">All Status</option>
            @foreach (var s in InternshipFormViewModel.StatusList)
            { <option value="@s" @(ViewBag.Filter_Status==s?"selected":"")>@s</option> }
        </select>
    </div>
    <div class="col-auto">
        <select name="internshipType" class="form-select form-select-sm">
            <option value="">All Types</option>
            @foreach (var t in InternshipFormViewModel.TypeList)
            { <option value="@t" @(ViewBag.Filter_InternshipType==t?"selected":"")>@t</option> }
        </select>
    </div>
    <div class="col-auto">
        <select name="department" class="form-select form-select-sm">
            <option value="">All Departments</option>
            @foreach (var d in ViewBag.Departments as List<string> ?? new())
            { <option value="@d" @(ViewBag.Filter_Department==d?"selected":"")>@d</option> }
        </select>
    </div>
    <div class="col-auto">
        <input name="company" type="text" class="form-control form-control-sm"
               placeholder="Company..." value="@ViewBag.Filter_Company"/>
    </div>
    <div class="col-auto d-flex gap-1">
        <button type="submit" class="btn btn-primary btn-sm">Filter</button>
        <a asp-action="Internships" class="btn btn-outline-secondary btn-sm">Clear</a>
    </div>
</form>
</div>
</div>

<!-- Summary Row -->
<div class="row g-2 mb-3">
@{
    var ppo = Model.Count(a => a.IsPPOConverted);
    var sel = Model.Count(a => a.Status == "Selected");
    var cmp = Model.Count(a => a.Status == "Completed");
}
<div class="col-auto">
    <span class="badge bg-info fs-6">Total: @Model.Count</span>
</div>
<div class="col-auto">
    <span class="badge bg-success fs-6">Selected: @sel</span>
</div>
<div class="col-auto">
    <span class="badge bg-teal fs-6">Completed: @cmp</span>
</div>
<div class="col-auto">
    <span class="badge badge-ppo fs-6">PPO: @ppo</span>
</div>
</div>

<div class="card border-0 shadow-sm">
<div class="table-responsive">
<table class="table table-hover table-sm mb-0">
    <thead class="table-light">
    <tr>
        <th>Student</th><th>Dept</th><th>Company</th><th>Role</th>
        <th>Type</th><th>Mode</th><th>Status</th>
        <th>Stipend</th><th>PPO</th><th>Updated</th>
    </tr>
    </thead>
    <tbody>
    @foreach (var a in Model)
    {
    <tr>
        <td class="fw-semibold small">@a.Student?.FullName</td>
        <td class="small text-muted">@a.Student?.Department</td>
        <td>@a.CompanyName</td>
        <td class="small">@a.Role</td>
        <td><span class="badge bg-light text-dark border">@a.InternshipType</span></td>
        <td><small class="text-muted">@a.WorkMode</small></td>
        <td><span class="badge badge-@a.Status">@a.Status</span></td>
        <td class="small">@(a.Stipend != null ? $"₹{a.Stipend}" : "—")</td>
        <td>
            @if (a.IsPPOConverted) {
                <span class="badge badge-ppo"><i class="bi bi-star-fill"></i></span>
            } else { <span class="text-muted">—</span> }
        </td>
        <td class="small text-muted">@a.UpdatedAt.ToString("dd MMM")</td>
    </tr>
    }
    </tbody>
</table>
</div>
</div>
```

---

### FILE: `Views/Account/AccessDenied.cshtml`
```html
@{ Layout = null; }
<!DOCTYPE html>
<html><head><meta charset="utf-8"/><title>Access Denied</title>
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css"/>
</head>
<body class="d-flex align-items-center justify-content-center" style="min-height:100vh;background:#f4f6fb;">
<div class="text-center">
    <h1 class="display-1 fw-bold text-danger">403</h1>
    <h4>Access Denied</h4>
    <p class="text-muted">You don't have permission to view this page.</p>
    <a href="/" class="btn btn-primary">Go Home</a>
</div>
</body></html>
```

---

## ═══════════════════════════════
## SECTION 9 — MIGRATIONS
## ═══════════════════════════════

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## ═══════════════════════════════
## SECTION 10 — README.md
## ═══════════════════════════════

```markdown
# 🎓 PlacementHub — Placement & Internship Tracker

## Tech Stack
- ASP.NET MVC 8.0 | EF Core 8 | SQL Server | Bootstrap 5 | Chart.js

## Setup
1. Clone repo
2. Update `appsettings.json` connection string
3. Run: `dotnet ef database update`
4. Run: `dotnet run`
5. Open: `https://localhost:xxxx`

## Demo Accounts
| Role | Email | Password |
|------|-------|----------|
| Student | rahul@student.com | Student@123 |
| Faculty | faculty@placement.com | Faculty@123 |
| Admin | admin@placement.com | Admin@123 |
| SuperAdmin | superadmin@placement.com | Admin@123 |

## Features

### 💼 Placement Tracking
- Full CRUD for job applications
- Status flow: Applied → Screening → Interview → Offer | Rejected | Withdrawn
- Interview scheduling with outcome tracking (Cleared / Not Cleared / Pending)
- Faculty remarks per application
- Filter by status / company / date
- Export to Excel

### 🏢 Internship Tracking ✦ NEW
- Full CRUD for internship applications (separate from placements)
- **Application Types**: Summer, Winter, Industrial Training, Research, PPO, Part-Time
- **Work Modes**: Remote, On-Site, Hybrid
- **Status Flow**: Applied → Shortlisted → Test → Interview → Selected | Rejected | Withdrawn | Completed
- Duration tracking (Start Date → End Date → auto-computed days)
- Monthly stipend recording in ₹
- **PPO Conversion Tracking** — mark when internship converts to a pre-placement offer
- **Certificate Received** flag for completed internships
- "Ongoing" internship badge on dashboard
- Internship interview rounds with outcome tracking
- Faculty remarks on internship applications
- Internship summary cards on Student Dashboard
- Admin view — all student internships with filters + Excel export
- Filter by status / type / mode / company / date

### 👩‍🏫 Faculty
- Track placement and internship progress side-by-side per student
- Add remarks to both placement and internship applications
- See upcoming placement AND internship interviews for all mentored students

### 🛡️ Admin / SuperAdmin
- Analytics dashboard with 5 placement KPI cards + 5 internship KPI cards
- Chart.js doughnuts for placement status, internship status, internship types
- Horizontal bar charts for top companies in both pipelines
- Export separate Excel files for placements and internships
- Companies table with `OffersInternships` / `OffersPlacement` flags
```

---

## ═══════════════════════════════
## SECTION 11 — GENERATION ORDER
## ═══════════════════════════════

Generate files in this EXACT order to avoid dependency errors:

1.  `Models/ApplicationUser.cs`
2.  `Models/JobApplication.cs`
3.  `Models/InternshipApplication.cs`          ← NEW
4.  `Models/InternshipInterview.cs`             ← NEW
5.  `Models/InterviewSchedule.cs`
6.  `Models/FacultyRemark.cs`
7.  `Models/FacultyStudentMapping.cs`
8.  `Models/Company.cs`
9.  `Models/Notification.cs`
10. `ViewModels/LoginViewModel.cs`
11. `ViewModels/RegisterViewModel.cs`
12. `ViewModels/ApplicationFormViewModel.cs`
13. `ViewModels/InternshipFormViewModel.cs`     ← NEW
14. `ViewModels/DashboardViewModel.cs`
15. `ViewModels/AdminDashboardViewModel.cs`
16. `ViewModels/FacultyDashboardViewModel.cs`
17. `Data/AppDbContext.cs`
18. `Data/SeedData.cs`
19. `Services/ApplicationService.cs`
20. `Services/InternshipService.cs`             ← NEW
21. `Services/AnalyticsService.cs`
22. `Services/NotificationService.cs`
23. `Controllers/AccountController.cs`
24. `Controllers/DashboardController.cs`
25. `Controllers/ApplicationsController.cs`
26. `Controllers/InternshipsController.cs`      ← NEW
27. `Controllers/FacultyController.cs`
28. `Controllers/AdminController.cs`
29. `Controllers/SuperAdminController.cs`
30. `Program.cs`
31. `Views/Shared/_Layout.cshtml`
32. `Views/Account/Login.cshtml`
33. `Views/Account/AccessDenied.cshtml`
34. `Views/Dashboard/Index.cshtml`
35. `Views/Applications/Index.cshtml`
36. `Views/Applications/Create.cshtml`
37. `Views/Applications/Edit.cshtml`
38. `Views/Applications/Details.cshtml`
39. `Views/Internships/Index.cshtml`            ← NEW
40. `Views/Internships/Create.cshtml`           ← NEW
41. `Views/Internships/Edit.cshtml`             ← NEW
42. `Views/Internships/Details.cshtml`          ← NEW
43. `Views/Faculty/Index.cshtml`
44. `Views/Faculty/StudentDetails.cshtml`
45. `Views/Admin/Index.cshtml`
46. `Views/Admin/Users.cshtml`
47. `Views/Admin/Applications.cshtml`
48. `Views/Admin/Internships.cshtml`            ← NEW
49. `Views/Admin/Companies.cshtml`
50. `Views/SuperAdmin/Index.cshtml`
51. `README.md`

---

## ═══════════════════════════════
## COMPLETE — BUILD & RUN
## ═══════════════════════════════

```bash
dotnet build
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Open browser → `https://localhost:xxxx` → Login as rahul@student.com / Student@123

> **Internship module is accessible from the left sidebar under the "Internship" section.**
> Demo data includes 3 internship applications for Rahul (Flipkart completed, Microsoft ongoing, DRDO applied)
> and 1 PPO-converted internship for Priya (TCS Winter → PPO to Infosys).
