using System.Collections.Generic;

namespace Titan_BugTracker.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<Project> AllProjects { get; set; }
        public List<Project> MyProjects { get; set; }
    }
}