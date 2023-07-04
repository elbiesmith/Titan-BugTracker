using System.Collections.Generic;

namespace Titan_BugTracker.Models.ViewModels
{
    public class AllProjectsViewModel
    {
        public List<Project> Projects { get; set; }
        public List<string> ProjectManagers { get; set; }
    }
}