using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Titan_BugTracker.Models.ViewModels
{
    public class PMIndexViewModel
    {
        public List<Project> Projects { get; set; }
        public SelectList PMList { get; set; }
        public string PmId { get; set; }
    }
}