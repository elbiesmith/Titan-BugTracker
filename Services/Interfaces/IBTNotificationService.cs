using System.Collections.Generic;
using System.Threading.Tasks;
using Titan_BugTracker.Models;

namespace Titan_BugTracker.Services.Interfaces
{
    public interface IBTNotificationService
    {
        public Task AddNotificationAsync(Notification notification);

        public Task<List<Notification>> GetReceivedNotificationsAsync(string userId);

        public Task<List<Notification>> GetSentNotificationsAsync(string userId);

        public Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role);

        public Task SendMembersEmailNotificationsAsync(Notification notification, List<BTUser> members);

        public Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject);

        public Task MarkNotificationAsRead(int id);
    }
}