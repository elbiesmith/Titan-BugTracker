using Microsoft.AspNetCore.Mvc.Rendering;

namespace Titan_BugTracker.Models.ViewModels
{
    public class AddProjectWithPMViewModel
    {
        public Project Project { get; set; } = new();
        public SelectList PMList { get; set; }
        public string PmId { get; set; }
        public SelectList PriorityList { get; set; }
        public int ProjectPriority { get; set; }
    }
}