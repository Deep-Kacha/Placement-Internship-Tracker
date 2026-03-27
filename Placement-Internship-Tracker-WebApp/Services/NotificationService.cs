using Microsoft.EntityFrameworkCore;
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
