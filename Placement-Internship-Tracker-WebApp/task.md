# Placement & Internship Tracker — Full Implementation

## Phase 1: Project Setup
- [ ] Update [.csproj](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Placement-Internship-Tracker-WebApp.csproj) with NuGet packages (EF Core, Identity, ClosedXML) and namespace
- [ ] Update [appsettings.json](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/appsettings.json) with connection string

## Phase 2: Models (9 files)
- [ ] `Models/ApplicationUser.cs`
- [ ] `Models/JobApplication.cs`
- [ ] `Models/InternshipApplication.cs`
- [ ] `Models/InternshipInterview.cs`
- [ ] `Models/InterviewSchedule.cs`
- [ ] `Models/FacultyRemark.cs`
- [ ] `Models/FacultyStudentMapping.cs`
- [ ] `Models/Company.cs`
- [ ] `Models/Notification.cs`

## Phase 3: ViewModels (6 files)
- [ ] `ViewModels/LoginViewModel.cs`
- [ ] `ViewModels/RegisterViewModel.cs`
- [ ] `ViewModels/ApplicationFormViewModel.cs`
- [ ] `ViewModels/InternshipFormViewModel.cs`
- [ ] `ViewModels/DashboardViewModel.cs`
- [ ] `ViewModels/AdminDashboardViewModel.cs`
- [ ] `ViewModels/FacultyDashboardViewModel.cs`

## Phase 4: Data Layer (2 files)
- [ ] `Data/AppDbContext.cs`
- [ ] `Data/SeedData.cs`

## Phase 5: Services (4 files)
- [ ] `Services/ApplicationService.cs`
- [ ] `Services/InternshipService.cs`
- [ ] `Services/AnalyticsService.cs`
- [ ] `Services/NotificationService.cs`

## Phase 6: Controllers (6 files)
- [ ] `Controllers/AccountController.cs`
- [ ] `Controllers/DashboardController.cs`
- [ ] `Controllers/ApplicationsController.cs`
- [ ] `Controllers/InternshipsController.cs`
- [ ] `Controllers/FacultyController.cs`
- [ ] `Controllers/AdminController.cs`
- [ ] `Controllers/SuperAdminController.cs`

## Phase 7: Program.cs
- [ ] Update [Program.cs](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Program.cs) with Identity, EF Core, Services, Cookie config

## Phase 8: Views — Shared & Account
- [ ] [Views/_ViewImports.cshtml](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Views/_ViewImports.cshtml) — update namespace
- [ ] [Views/Shared/_Layout.cshtml](file:///d:/Placement-Internship-Tracker/Placement-Internship-Tracker-WebApp/Views/Shared/_Layout.cshtml) — full sidebar layout
- [ ] `Views/Account/Login.cshtml`
- [ ] `Views/Account/Register.cshtml`
- [ ] `Views/Account/AccessDenied.cshtml`

## Phase 9: Views — Student
- [ ] `Views/Dashboard/Index.cshtml`
- [ ] `Views/Applications/Index.cshtml`
- [ ] `Views/Applications/Create.cshtml`
- [ ] `Views/Applications/Edit.cshtml`
- [ ] `Views/Applications/Details.cshtml`
- [ ] `Views/Internships/Index.cshtml`
- [ ] `Views/Internships/Create.cshtml`
- [ ] `Views/Internships/Edit.cshtml`
- [ ] `Views/Internships/Details.cshtml`

## Phase 10: Views — Faculty & Admin
- [ ] `Views/Faculty/Index.cshtml`
- [ ] `Views/Faculty/StudentDetails.cshtml`
- [ ] `Views/Admin/Index.cshtml`
- [ ] `Views/Admin/Users.cshtml`
- [ ] `Views/Admin/Applications.cshtml`
- [ ] `Views/Admin/Internships.cshtml`
- [ ] `Views/Admin/Companies.cshtml`
- [ ] `Views/Admin/CreateStudent.cshtml`
- [ ] `Views/Admin/CreateFaculty.cshtml`
- [ ] `Views/SuperAdmin/Index.cshtml`

## Phase 11: Build & Migrate
- [ ] Run `dotnet restore`
- [ ] Run `dotnet build`
- [ ] Run EF migrations
- [ ] Run and verify app

## Phase 12: README
- [ ] Create `README.md`
