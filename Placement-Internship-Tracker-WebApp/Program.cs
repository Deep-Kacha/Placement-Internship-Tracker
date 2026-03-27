using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;
using PlacementTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// ─── EF Core + SQL Server ───
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── Identity ───
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ─── Cookie Auth ───
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

// ─── Services DI ───
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<InternshipService>();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<NotificationService>();

// ─── MVC ───
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ─── Database Migration + Seed Data ───
using (var scope = app.Services.CreateScope())
{
    var svc = scope.ServiceProvider;
    try
    {
        var db = svc.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        await SeedData.InitializeAsync(svc);
    }
    catch (Exception ex)
    {
        var logger = svc.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating/seeding the database.");
    }
}

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
