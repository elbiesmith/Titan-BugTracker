using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Titan_BugTracker.Models.ViewModels
{
    public class ProjectDetailsViewModel
    {
        public Project Project { get; set; }

        public BTUser ProjectManager { get; set; }
    }
}