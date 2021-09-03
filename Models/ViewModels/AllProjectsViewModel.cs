using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Titan_BugTracker.Models.ViewModels
{
    public class AllProjectsViewModel
    {
        public List<Project> Projects { get; set; }
        public List<string> ProjectManagers { get; set; }
    }
}