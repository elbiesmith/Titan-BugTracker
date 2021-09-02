using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Titan_BugTracker.Models.ViewModels
{
    public class AddProjectWithPMViewModel
    {
        public Project project { get; set; } = new();
        public SelectList PMList { get; set; }
        public string PmId { get; set; }
        public SelectList PriorityList { get; set; }
        public int ProjectPriority { get; set; }
    }
}