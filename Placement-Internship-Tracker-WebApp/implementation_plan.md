# Placement & Internship Tracker — Full Implementation Plan

Build a complete, production-ready ASP.NET MVC 8.0 application per the master prompt ([PlacementTracker_FULL_Implementation_Prompt_v2.md](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/PlacementTracker_FULL_Implementation_Prompt_v2.md)). The app uses Identity for authentication, EF Core with SQL Server (LocalDB), Bootstrap 5 UI with RKU-inspired theme colors, and Chart.js for analytics.

## User Review Required

> [!IMPORTANT]
> This is a massive project (~51 files). The namespace will be changed from `Placement_Internship_Tracker_WebApp` to `PlacementTracker` to match the master prompt's conventions. The existing [HomeController.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Controllers/HomeController.cs) and [Models/ErrorViewModel.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Models/ErrorViewModel.cs) will be removed as they're replaced by the new project structure.

> [!WARNING]
> The RKU website theme uses a deep blue/maroon color scheme. We'll incorporate this into the sidebar gradient (`#1a1f36 → #2d3561`) as specified in the master prompt, with accent colors inspired by RKU branding.

## Proposed Changes

### Project Configuration

#### [MODIFY] [Placement-Internship-Tracker-WebApp.csproj](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Placement-Internship-Tracker-WebApp.csproj)
- Change `RootNamespace` to `PlacementTracker`
- Add NuGet package references: EF Core SqlServer, Identity, ClosedXML, EF Tools/Design

#### [MODIFY] [appsettings.json](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/appsettings.json)
- Add `DefaultConnection` connection string for LocalDB

---

### Models (9 files)

#### [MODIFY] [ApplicationUser.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Models/ApplicationUser.cs)
#### [NEW] [JobApplication.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Models/JobApplication.cs)
#### [NEW] [InternshipApplication.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Models/InternshipApplication.cs)
#### [NEW] [InternshipInterview.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Models/InternshipInterview.cs)
#### [NEW] [InterviewSchedule.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Models/InterviewSchedule.cs)
#### [NEW] [FacultyRemark.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Models/FacultyRemark.cs)
#### [NEW] [FacultyStudentMapping.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Models/FacultyStudentMapping.cs)
#### [NEW] [Company.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Models/Company.cs)
#### [NEW] [Notification.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Models/Notification.cs)
#### [DELETE] [ErrorViewModel.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Models/ErrorViewModel.cs)

All models as specified in the master prompt Section 2.

---

### ViewModels (7 files)

#### [NEW] [LoginViewModel.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/ViewModels/LoginViewModel.cs)
#### [NEW] [RegisterViewModel.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/ViewModels/RegisterViewModel.cs)
#### [NEW] [ApplicationFormViewModel.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/ViewModels/ApplicationFormViewModel.cs)
#### [NEW] [InternshipFormViewModel.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/ViewModels/InternshipFormViewModel.cs)
#### [NEW] [DashboardViewModel.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/ViewModels/DashboardViewModel.cs)
#### [NEW] [AdminDashboardViewModel.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/ViewModels/AdminDashboardViewModel.cs)
#### [NEW] [FacultyDashboardViewModel.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/ViewModels/FacultyDashboardViewModel.cs)

All ViewModels as specified in the master prompt Section 3.

---

### Data Layer (2 files)

#### [NEW] [AppDbContext.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Data/AppDbContext.cs)
#### [NEW] [SeedData.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Data/SeedData.cs)

EF Core context with full relationship configuration and seed data with demo accounts + sample data.

---

### Services (4 files)

#### [NEW] [ApplicationService.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Services/ApplicationService.cs)
#### [NEW] [InternshipService.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Services/InternshipService.cs)
#### [NEW] [AnalyticsService.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Services/AnalyticsService.cs)
#### [NEW] [NotificationService.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Services/NotificationService.cs)

All services as specified in the master prompt Section 5.

---

### Controllers (7 files)

#### [NEW] [AccountController.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Controllers/AccountController.cs)
#### [NEW] [DashboardController.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Controllers/DashboardController.cs)
#### [NEW] [ApplicationsController.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Controllers/ApplicationsController.cs)
#### [NEW] [InternshipsController.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Controllers/InternshipsController.cs)
#### [NEW] [FacultyController.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Controllers/FacultyController.cs)
#### [NEW] [AdminController.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Controllers/AdminController.cs)
#### [NEW] [SuperAdminController.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Controllers/SuperAdminController.cs)
#### [DELETE] [HomeController.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Controllers/HomeController.cs)

---

### Program.cs & Config

#### [MODIFY] [Program.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Program.cs)

Replace with full configuration: EF Core + SQL Server, Identity, cookie auth, service registration, seed data call.

---

### Views (~25 files)

#### [MODIFY] [_ViewImports.cshtml](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Views/_ViewImports.cshtml)
#### [MODIFY] [_Layout.cshtml](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Views/Shared/_Layout.cshtml)
#### [NEW] Views for Account (Login, Register, AccessDenied)
#### [NEW] Views for Dashboard (Index)
#### [NEW] Views for Applications (Index, Create, Edit, Details)
#### [NEW] Views for Internships (Index, Create, Edit, Details)
#### [NEW] Views for Faculty (Index, StudentDetails)
#### [NEW] Views for Admin (Index, Users, Applications, Internships, Companies, CreateStudent, CreateFaculty)
#### [NEW] Views for SuperAdmin (Index)

All views with Bootstrap 5 + sidebar layout, status badge colors, Chart.js charts for admin dashboard.

---

## Verification Plan

### Automated Tests
1. **Build verification**: `dotnet build` — must complete with 0 errors
2. **Database migration**: `dotnet ef migrations add InitialCreate` + `dotnet ef database update`

### Manual Verification
1. Run `dotnet run` from the project directory
2. Open browser to the displayed localhost URL
3. Login as `rahul@student.com` / `Student@123` → verify student dashboard loads with placement & internship summary cards
4. Navigate to "Job Applications" → verify seed data (Google, Microsoft, Amazon)
5. Navigate to "My Internships" → verify seed data (Flipkart, Microsoft, DRDO)
6. Logout, login as `admin@placement.com` / `Admin@123` → verify admin analytics dashboard with charts
7. Verify "All Internships" admin view loads with filterable table
