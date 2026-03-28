using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using PlacementTracker.Data;
using PlacementTracker.Models;

namespace PlacementTracker.Services
{
    public class NotificationService
    {
        private readonly AppDbContext _db;
        private readonly IEmailSender _emailSender;

        public NotificationService(AppDbContext db, IEmailSender emailSender) 
        { 
            _db = db; 
            _emailSender = emailSender;
        }

        public async Task SendAsync(string userId, string message, string? link = null)
        {
            _db.Notifications.Add(new Notification {
                UserId = userId, Message = message, Link = link
            });
            await _db.SaveChangesAsync();

            var user = await _db.Users.FindAsync(userId);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                string host = "http://localhost:5069"; // Localhost for dev
                string htmlMsg = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; padding: 20px; border: 1px solid #e5e7eb; border-radius: 12px;'>
                    <h2 style='color: #B91C1C; margin-top: 0;'>Trackeoo Notification</h2>
                    <p style='font-size: 16px; color: #374151;'>Hello {user.FullName},</p>
                    <p style='font-size: 16px; color: #4B5563;'>You have a new update regarding your placements and internships:</p>
                    <div style='background-color: #fef2f2; padding: 15px; border-left: 4px solid #B91C1C; margin: 20px 0; border-radius: 0 8px 8px 0;'>
                        <p style='margin: 0; font-size: 16px; font-weight: bold; color: #7F1D1D;'>{message}</p>
                    </div>";

                if (!string.IsNullOrEmpty(link))
                {
                    htmlMsg += $@"<p><a href='{host}{link}' style='display: inline-block; padding: 12px 24px; background-color: #B91C1C; color: white; text-decoration: none; border-radius: 8px; font-weight: bold;'>View Details</a></p>";
                }

                htmlMsg += "<p style='font-size: 13px; color: #9CA3AF; margin-top: 30px; border-top: 1px solid #e5e7eb; padding-top: 15px;'>Thank you,<br/>Trackeoo Training & Placement Cell</p></div>";

                try {
                    await _emailSender.SendEmailAsync(user.Email, "New Update from Trackeoo", htmlMsg);
                } 
                catch 
                { 
                    // Catch block prevents application crash if SMTP server is down/misconfigured
                }
            }
        }

        public async Task<List<Notification>> GetNotificationsAsync(string userId)
        {
            return await _db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkReadAsync(int id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n != null) { n.IsRead = true; await _db.SaveChangesAsync(); }
        }

        public async Task DeleteAsync(int id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n != null) { _db.Notifications.Remove(n); await _db.SaveChangesAsync(); }
        }
    }
}
