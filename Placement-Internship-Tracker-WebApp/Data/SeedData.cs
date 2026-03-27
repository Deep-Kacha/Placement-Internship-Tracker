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

            // Database is already migrated by Program.cs

            string[] roles = { "SuperAdmin", "Admin", "Faculty", "Student", "Recruiter" };
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            await CreateUser(userManager, new ApplicationUser {
                UserName = "superadmin@placement.com",
                Email = "superadmin@placement.com",
                FullName = "Super Admin",
                Department = "Management",
                EmailConfirmed = true, IsApproved = true
            }, "Admin@123", "SuperAdmin");

            var admin = await CreateUser(userManager, new ApplicationUser {
                UserName = "admin@placement.com",
                Email = "admin@placement.com",
                FullName = "Placement Admin",
                Department = "Placement Cell",
                EmailConfirmed = true, IsApproved = true
            }, "Admin@123", "Admin");

            var faculty = await CreateUser(userManager, new ApplicationUser {
                UserName = "faculty@placement.com",
                Email = "faculty@placement.com",
                FullName = "Prof. Anjali Mehta",
                Department = "Computer Science",
                EmailConfirmed = true, IsApproved = true
            }, "Faculty@123", "Faculty");

            var student1 = await CreateUser(userManager, new ApplicationUser {
                UserName = "rahul@student.com",
                Email = "rahul@student.com",
                FullName = "Rahul Sharma",
                Department = "Computer Science",
                CollegeRollNo = "CS2021001",
                Branch = "CE", Semester = "8",
                CGPA = 8.5, TenthPercentage = 85, TwelfthPercentage = 82, DiplomaPercentage = 0,
                CurrentAddress = "123, University Road", City = "Rajkot", State = "Gujarat", Pincode = "360020",
                ParentName = "Rajesh Kacha", ParentMobile = "9876543210", ParentEmail = "parent@example.com",
                EmailConfirmed = true, IsApproved = true
            }, "Student@123", "Student");

            var student2 = await CreateUser(userManager, new ApplicationUser {
                UserName = "priya@student.com",
                Email = "priya@student.com",
                FullName = "Priya Patel",
                Department = "Information Technology",
                CollegeRollNo = "IT2021015",
                Branch = "IT", Semester = "8",
                CGPA = 9.2, TenthPercentage = 92, TwelfthPercentage = 88,
                EmailConfirmed = true, IsApproved = true
            }, "Student@123", "Student");

            var recruiter = await CreateUser(userManager, new ApplicationUser {
                UserName = "recruiter@google.com",
                Email = "recruiter@google.com",
                FullName = "Sundar Pichai",
                Department = "Talent Acquisition",
                EmailConfirmed = true, IsApproved = true
            }, "Recruiter@123", "Recruiter");

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

            var recruiterAcc = await userManager.FindByEmailAsync("recruiter@google.com");
            if (!context.JobDescriptions.Any())
            {
                context.JobDescriptions.AddRange(
                    new JobDescription {
                        RecruiterId = recruiterAcc?.Id, Title = "SDE Intern", CompanyName = "Google", JobType = "Internship",
                        Location = "Bangalore", PackageOrStipend = "80,000/month",
                        AnnualCTC = "12 LPA", Bond = "None", SelectionProcess = "OA + 3 Technical Rounds",
                        EligibleBatches = "2024, 2025", EligibleCourses = "B.Tech CE/IT",
                        Description = "Join Google's Core Search team to build distributed systems that handle billions of queries per day.\n\nYou will work with senior engineers to implement caching algorithms and optimize search latency globally.",
                        Requirements = "• Strong algorithms and data structures\n• Proficient in C++ or Java\n• Available for 6 months continuously",
                        Deadline = DateTime.Now.AddDays(10), CampusDriveDate = DateTime.Now.AddDays(15),
                        IsActive = true, Status = "Approved", CreatedAt = DateTime.Now.AddDays(-15)
                    },
                    new JobDescription {
                        RecruiterId = recruiterAcc?.Id, Title = "Cloud Engineer", CompanyName = "Microsoft", JobType = "Placement",
                        Location = "Hyderabad", PackageOrStipend = "18 LPA",
                        AnnualCTC = "18 LPA", Bond = "1 Year", SelectionProcess = "OA + Group Discussion + Technical + HR",
                        EligibleBatches = "2024", EligibleCourses = "B.Tech All Branches",
                        Description = "Work with Azure infrastructure and deploy planet-scale cloud services. Ensure high availability of critical tenant workloads.",
                        Requirements = "• B.Tech in Computer Science\n• Knowledge of OS, Virtualization, and Cloud computing",
                        Deadline = DateTime.Now.AddDays(5), CampusDriveDate = DateTime.Now.AddDays(8),
                        IsActive = true, Status = "Approved", CreatedAt = DateTime.Now.AddDays(-10)
                    },
                    new JobDescription {
                        RecruiterId = recruiterAcc?.Id, Title = "SDE-1", CompanyName = "Amazon", JobType = "Placement",
                        Location = "Bangalore", PackageOrStipend = "24 LPA",
                        AnnualCTC = "24 LPA", Bond = "None", SelectionProcess = "2 Technical + 1 Managerial Round",
                        EligibleBatches = "2024", EligibleCourses = "B.Tech CE/IT/EC",
                        Description = "Build backend services for Amazon Retail. You will work on AWS heavily to scale up our order fulfillment pipelines.",
                        Requirements = "• High problem solving skills\n• Object Oriented Design understanding",
                        Deadline = DateTime.Now.AddDays(2), CampusDriveDate = DateTime.Now.AddDays(4),
                        IsActive = true, Status = "Approved", CreatedAt = DateTime.Now.AddDays(-5)
                    },
                    new JobDescription {
                        RecruiterId = recruiterAcc?.Id, Title = "Research Intern", CompanyName = "Microsoft", JobType = "Internship",
                        Location = "Remote", PackageOrStipend = "40000",
                        AnnualCTC = "N/A", Bond = "None", SelectionProcess = "Resume Shortlisting + Interview",
                        EligibleBatches = "2025, 2026", EligibleCourses = "B.Tech / M.Tech / PhD",
                        Description = "Work on cutting edge NLP and Large Language Models inside Microsoft Research. Publish papers and optimize transformer models.",
                        Requirements = "• Python, PyTorch, Deep Learning fundamentals\n• Previous research experience is a plus",
                        Deadline = DateTime.Now.AddDays(12), CampusDriveDate = DateTime.Now.AddDays(20),
                        IsActive = true, Status = "Approved", CreatedAt = DateTime.Now.AddDays(-2)
                    },
                    new JobDescription {
                        RecruiterId = recruiterAcc?.Id, Title = "Software Developer", CompanyName = "Flipkart", JobType = "Placement",
                        Location = "Bangalore", PackageOrStipend = "15 LPA",
                        Description = "Revamp the cart and checkout experience for millions of users during the Big Billion Days.",
                        Requirements = "• React.js, Node.js\n• Basic System Design",
                        Deadline = DateTime.Now.AddDays(20), IsActive = true, Status = "Pending", CreatedAt = DateTime.Now.AddDays(-1)
                    },
                    new JobDescription {
                        RecruiterId = recruiterAcc?.Id, Title = "IT Analyst Intern", CompanyName = "TCS", JobType = "Internship",
                        Location = "Pune", PackageOrStipend = "10000",
                        Description = "Winter internship program inside TCS. Best performing interns will receive a pre-placement offer.",
                        Requirements = "• Basic programming\n• Strong communication skills",
                        Deadline = DateTime.Now.AddDays(-1), IsActive = true, Status = "Approved", CreatedAt = DateTime.Now.AddMonths(-5)
                    }
                );
                await context.SaveChangesAsync();
            }

            if (student1 != null && !context.JobApplications.Any(a => a.StudentId == student1.Id))
            {
                var jdGoogle = context.JobDescriptions.FirstOrDefault(j => j.CompanyName == "Google" && j.Title.Contains("SDE Intern"));
                var jdMs = context.JobDescriptions.FirstOrDefault(j => j.CompanyName == "Microsoft" && j.Title.Contains("Cloud"));
                var jdAzn = context.JobDescriptions.FirstOrDefault(j => j.CompanyName == "Amazon");

                var app1 = new JobApplication {
                    StudentId = student1.Id, JobDescriptionId = jdGoogle?.Id, CompanyName = "Google", Role = "SDE Intern",
                    Status = "Offer", AppliedDate = DateTime.Now.AddDays(-30),
                    Package = "80,000/month", Location = "Bangalore",
                    Notes = "Referred by alumni.", UpdatedAt = DateTime.Now.AddDays(-5)
                };
                var app2 = new JobApplication {
                    StudentId = student1.Id, JobDescriptionId = jdMs?.Id, CompanyName = "Microsoft", Role = "Cloud Engineer",
                    Status = "Interview", AppliedDate = DateTime.Now.AddDays(-15),
                    JobLink = "https://careers.microsoft.com", Location = "Hyderabad",
                    UpdatedAt = DateTime.Now.AddDays(-2)
                };
                var app3 = new JobApplication {
                    StudentId = student1.Id, JobDescriptionId = jdAzn?.Id, CompanyName = "Amazon", Role = "SDE-1",
                    Status = "Applied", AppliedDate = DateTime.Now.AddDays(-5),
                    UpdatedAt = DateTime.Now.AddDays(-5)
                };
                context.JobApplications.AddRange(app1, app2, app3);
                await context.SaveChangesAsync();

                context.InterviewSchedules.Add(new InterviewSchedule {
                    JobApplicationId = app2.Id, InterviewType = "Technical",
                    InterviewDate = DateTime.Now.AddDays(3),
                    InterviewTime = new TimeSpan(10, 0, 0),
                    MeetingLink = "https://teams.microsoft.com/meet/abc123",
                    Outcome = "Pending"
                });

                if (faculty != null)
                    context.FacultyRemarks.Add(new FacultyRemark {
                        FacultyId = faculty.Id, JobApplicationId = app2.Id,
                        Comment = "Good progress Rahul! Prepare system design thoroughly for Microsoft round."
                    });

                if (faculty != null && !context.FacultyStudentMappings.Any(m => m.StudentId == student1.Id))
                    context.FacultyStudentMappings.Add(new FacultyStudentMapping {
                        FacultyId = faculty.Id, StudentId = student1.Id
                    });

                await context.SaveChangesAsync();
            }

            if (student1 != null && !context.InternshipApplications.Any(a => a.StudentId == student1.Id))
            {
                var jdFlipkart = context.JobDescriptions.FirstOrDefault(j => j.CompanyName == "Flipkart");
                var jdMsResearch = context.JobDescriptions.FirstOrDefault(j => j.CompanyName == "Microsoft" && j.Title.Contains("Research"));

                var intern1 = new InternshipApplication {
                    StudentId = student1.Id, JobDescriptionId = jdFlipkart?.Id, CompanyName = "Flipkart", Role = "SDE Intern",
                    InternshipType = "Summer", WorkMode = "Hybrid", Status = "Completed",
                    AppliedDate = DateTime.Now.AddMonths(-5), StartDate = DateTime.Now.AddMonths(-3),
                    EndDate = DateTime.Now.AddMonths(-1), Stipend = "25000", Location = "Bangalore",
                    CertificateReceived = true, IsFullTimeOffered = false,
                    Notes = "Worked on search ranking improvements.", UpdatedAt = DateTime.Now.AddMonths(-1)
                };
                var intern2 = new InternshipApplication {
                    StudentId = student1.Id, JobDescriptionId = jdMsResearch?.Id, CompanyName = "Microsoft", Role = "Research Intern",
                    InternshipType = "Research", WorkMode = "Remote", Status = "Selected",
                    AppliedDate = DateTime.Now.AddDays(-20), StartDate = DateTime.Now.AddDays(10),
                    EndDate = DateTime.Now.AddDays(70), Stipend = "40000", Location = "Remote",
                    Notes = "NLP research internship with Azure AI team.", UpdatedAt = DateTime.Now.AddDays(-3)
                };
                var intern3 = new InternshipApplication {
                    StudentId = student1.Id, CompanyName = "DRDO", Role = "Project Trainee",
                    InternshipType = "Industrial Training", WorkMode = "On-Site", Status = "Applied",
                    AppliedDate = DateTime.Now.AddDays(-2), Stipend = "8000", Location = "Delhi",
                    Notes = "8-week mandatory industrial training.", UpdatedAt = DateTime.Now.AddDays(-2)
                };
                context.InternshipApplications.AddRange(intern1, intern2, intern3);
                await context.SaveChangesAsync();

                context.InternshipInterviews.Add(new InternshipInterview {
                    InternshipApplicationId = intern2.Id, RoundType = "Technical",
                    InterviewDate = DateTime.Now.AddDays(15),
                    InterviewTime = new TimeSpan(11, 30, 0),
                    MeetingLink = "https://teams.microsoft.com/meet/intern123",
                    Outcome = "Pending"
                });

                if (faculty != null)
                    context.FacultyRemarks.Add(new FacultyRemark {
                        FacultyId = faculty.Id, InternshipApplicationId = intern2.Id,
                        Comment = "Excellent opportunity Rahul! Brush up on transformers and NLP fundamentals."
                    });

                await context.SaveChangesAsync();
            }

            if (student2 != null && !context.JobApplications.Any(a => a.StudentId == student2.Id))
            {
                var jdInfosys = context.JobDescriptions.FirstOrDefault(j => j.CompanyName == "Infosys");
                var app = new JobApplication {
                    StudentId = student2.Id, JobDescriptionId = jdInfosys?.Id, CompanyName = "Infosys", Role = "System Engineer",
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

            if (student2 != null && !context.InternshipApplications.Any(a => a.StudentId == student2.Id))
            {
                var jdTcs = context.JobDescriptions.FirstOrDefault(j => j.CompanyName == "TCS");
                var intern = new InternshipApplication {
                    StudentId = student2.Id, JobDescriptionId = jdTcs?.Id, CompanyName = "TCS", Role = "IT Analyst Intern",
                    InternshipType = "Winter", WorkMode = "On-Site", Status = "Completed",
                    AppliedDate = DateTime.Now.AddMonths(-4), StartDate = DateTime.Now.AddMonths(-3),
                    EndDate = DateTime.Now.AddMonths(-2), Stipend = "10000", Location = "Pune",
                    CertificateReceived = true, IsFullTimeOffered = true, FullTimePackage = "3.6 LPA",
                    Notes = "Full-time offer extended after winter internship performance.", UpdatedAt = DateTime.Now.AddMonths(-2)
                };
                context.InternshipApplications.Add(intern);
                await context.SaveChangesAsync();
            }

            // Seed Notifications
            if (!context.Notifications.Any())
            {
                if (student1 != null)
                {
                    context.Notifications.AddRange(
                        new Notification { UserId = student1.Id, Message = "Your application for Google has been updated to 'Offer'!", CreatedAt = DateTime.Now.AddDays(-1), IsRead = false, Link = "/Applications/Index" },
                        new Notification { UserId = student1.Id, Message = "New Job Posting: Research Intern at Microsoft matches your profile.", CreatedAt = DateTime.Now.AddDays(-2), IsRead = true, Link = "/JobBoard/Index" },
                        new Notification { UserId = student1.Id, Message = "Faculty Remark: Prof. Anjali added a comment on your Microsoft application.", CreatedAt = DateTime.Now.AddHours(-5), IsRead = false, Link = "/Applications/Index" }
                    );
                }
                if (admin != null)
                {
                    context.Notifications.AddRange(
                        new Notification { UserId = admin.Id, Message = "New Student Registration: Priya Patel is pending approval.", CreatedAt = DateTime.Now.AddDays(-1), IsRead = false, Link = "/Admin/Users" },
                        new Notification { UserId = admin.Id, Message = "JD Submitted: Google posted a new 'SDE Intern' position.", CreatedAt = DateTime.Now.AddHours(-2), IsRead = false, Link = "/Admin/JobDescriptions" }
                    );
                }
                await context.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUser?> CreateUser(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user, string password, string role)
        {
            var existingUser = await userManager.FindByEmailAsync(user.Email!);
            if (existingUser != null) {
                existingUser.IsActive = true;
                existingUser.IsApproved = true;
                await userManager.UpdateAsync(existingUser);
                return existingUser;
            }
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
            return result.Succeeded ? user : null;
        }
    }
}
