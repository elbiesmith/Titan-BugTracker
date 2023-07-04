using System.Collections.Generic;

namespace Titan_BugTracker.Models.ViewModels
{
    public class NotificationsViewModel
    {
        public List<Notification> SentNotifications { get; set; }

        public List<Notification> RecievedNotifications { get; set; }
    }
}