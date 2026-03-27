# Trackeoo - Placement & Internship Tracker

## Overview
**Trackeoo** is a comprehensive, modern web application designed to streamline the campus placement and internship tracking process for universities and colleges. It serves as a centralized platform connecting Students, Faculty, Recruiters, and the Training & Placement (T&P) Administrative team.

Built with performance, security, and user experience in mind, the platform eliminates tedious manual tracking through Excel sheets and emails by providing dedicated role-based portals, automated application pipelines, and real-time dashboard analytics.

---

## Architecture & Technology Stack
*   **Backend Framework:** ASP.NET Core MVC 8.0 (C#)
*   **Database ORM:** Entity Framework Core
*   **Database:** Microsoft SQL Server
*   **Authentication & Authorization:** ASP.NET Core Identity (Cookie-based, Role-Based Access Control)
*   **Frontend Design System:** Tailwind CSS (Utility-first framework) + Custom CSS classes
*   **UI Components & Icons:** HTML5/Razor syntax, Bootstrap Icons (bi)

---

## Role-Based Access Control (RBAC)

Trackeoo enforces strict data visibility and capabilities based on 5 distinct user roles:

### 1. Student
*   **Profile Management:** Can maintain their academic profile (CGPA, roll numbers, percentages, resumes).
*   **Job Board Browser:** View and filter active, approved Job/Internship postings (JDs).
*   **Application Tracking:** Apply to postings with a single click and track real-time application status (Applied -> Shortlisted -> Interview -> Offer/Rejected).
*   **Dashboards:** See visual KPIs of their total applications, ongoing internships, and upcoming scheduled interview dates/links.

### 2. Administrator (T&P Officer)
*   **Total System Oversight:** Can view system-level dashboards (Placement Rates, Top Companies, Activity Feeds).
*   **User Management:** Approves pending student/faculty registrations to ensure data integrity.
*   **Job Management:** Acts as the gatekeeper. Reviews guest-submitted or recruiter-submitted Job Descriptions (JDs) and approves them before students can see them.
*   **Applicant Tracking Pipeline:** Moves students through the hiring funnel (Shortlisting resumes, scheduling interviews, extending offers).

### 3. SuperAdmin
*   Highest tier access. Possesses all Administrative rights plus the ability to manage Admin accounts, configure global platform settings, and access raw database diagnostics.

### 4. Recruiter
*   **Hiring Management:** Can post new Job Descriptions directly to the platform.
*   **Applicant Review:** Can review student profiles and resumes that apply to their specific company's postings.
*   **Scheduling:** Can schedule interviews and send meeting links directly through the platform.

### 5. Faculty
*   **Student Mentorship:** Can view profiles and applications of students mapped to their department/branch.
*   **Feedback/Remarks:** Can add internal remarks or guidance notes on a student's application to help them prepare for interviews or improve their resumes.

### 6. Guest User (Unauthenticated)
*   Can view the public-facing landing page features and roadmap.
*   **Public JD Submission:** Guest recruiters can submit a detailed Job Description (CTC, Stipend, Roles, Documents) without needing to register. These submissions land in the Admin's "Pending" queue for review.

---

## Core Features Implemented

1.  **Centralized Dashboards:** Dedicated dashboard views customized per role. Admins see global metrics; students see personal application funnels.
2.  **Universal Notification System:** Real-time, drop-down notification bells alert users of critical events (e.g., "Your application was shortlisted by Microsoft", "New JD submitted by Google").
3.  **Comprehensive Registration & Onboarding:** Secure, multi-step registration forms with built-in validation separating student info (Roll numbers, branches) from Faculty/Admin data.
4.  **Premium UI/UX:** A modern, minimal "brand red and cream" aesthetic heavily utilizing gradients, glassmorphism, responsive grids, and subtle hover animations tailored with Tailwind CSS.
5.  **Secure Document Handling:** Students can upload resumes and recruiters/admins can upload JD documents securely to the server.
6.  **Full Application Pipeline:** A robust State Machine tracks jobs from `Active` -> `Closed`, and applications from `Applied` -> `Interview Scheduled` -> `Offer` / `Rejected`.
7.  **Password Management:** Full secure implementation of "Forgot Password" to email-simulated reset token flows.

---

## Project Setup & Running Locally

1.  Ensure you have **.NET 8.0 SDK** and **SQL Server** installed.
2.  Clone the repository and open `Placement-Internship-Tracker-WebApp.csproj` in Visual Studio or VS Code.
3.  Ensure the `appsettings.json` connection string points to your local SQL Server instance.
4.  Run EF Core Migrations to create the schema (the app is configured to auto-migrate on startup).
5.  On the first run, the system automatically runs `SeedData.cs` to populate demo accounts, sample companies, real-world Job Descriptions, and dummy applications to test the UI immediately.
6.  Build and run the project using `dotnet run` or IIS Express.

### Demo Credentials (Pre-seeded):
*   **Student:** `rahul@student.com` | Password: `Student@123`
*   **Faculty:** `faculty@placement.com` | Password: `Faculty@123`
*   **Admin:** `admin@placement.com` | Password: `Admin@123`
*   **SuperAdmin:** `superadmin@placement.com` | Password: `Admin@123`
