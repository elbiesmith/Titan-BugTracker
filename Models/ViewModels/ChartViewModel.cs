using System.Collections.Generic;

namespace Titan_BugTracker.Models.ViewModels
{
    public class ChartViewModel
    {
        public List<Project> Projects { get; set; }

        public List<Ticket> Tickets { get; set; }

        public List<string> Labels { get; set; } = new();

        public List<int> DevelopmentTickets { get; set; } = new();

        public List<int> AllTickets { get; set; } = new();

        public List<int> TestingTickets { get; set; } = new();
    }
}