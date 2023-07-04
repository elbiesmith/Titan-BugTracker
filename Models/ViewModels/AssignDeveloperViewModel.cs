using Microsoft.AspNetCore.Mvc.Rendering;

namespace Titan_BugTracker.Models.ViewModels
{
    public class AssignDeveloperViewModel
    {
        public Project Project { get; set; }
        public SelectList Developers { get; set; }
        public string DeveloperId { get; set; }
        public Ticket Ticket { get; set; }
    }
}