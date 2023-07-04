using System.Collections.Generic;

namespace Titan_BugTracker.Models.ViewModels
{
    public class AdminDashboard
    {
        public List<Project> Projects { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}