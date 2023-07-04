using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Titan_BugTracker.Models.ViewModels
{
    public class PMIndexViewModel
    {
        public List<Project> Projects { get; set; }
        public SelectList PMList { get; set; }
        public string PmId { get; set; }
    }
}