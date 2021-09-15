using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Titan_BugTracker.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<Project> AllProjects { get; set; }
        public List<Project> MyProjects { get; set; }
    }
}