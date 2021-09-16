using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Titan_BugTracker.Models.ViewModels
{
    public class NotificationsViewModel
    {
        public List<Notification> SentNotifications { get; set; }

        public List<Notification> RecievedNotifications { get; set; }
    }
}