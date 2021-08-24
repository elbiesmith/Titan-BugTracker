using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Titan_BugTracker.Data;
using Titan_BugTracker.Models;

namespace Titan_BugTracker.Views
{
    public class IndexModel : PageModel
    {
        private readonly Titan_BugTracker.Data.ApplicationDbContext _context;

        public IndexModel(Titan_BugTracker.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Company> Company { get;set; }

        public async Task OnGetAsync()
        {
            Company = await _context.Companies.ToListAsync();
        }
    }
}
