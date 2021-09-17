using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Titan_BugTracker.Data;
using Titan_BugTracker.Extensions;
using Titan_BugTracker.Models;
using Titan_BugTracker.Models.Enums;
using Titan_BugTracker.Models.ViewModels;
using Titan_BugTracker.Services.Interfaces;

namespace Titan_BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _usermanager;
        private readonly ApplicationDbContext _context;

        public HomeController(IBTProjectService projectService, UserManager<BTUser> usermanager, ApplicationDbContext context)
        {
            _projectService = projectService;
            _usermanager = usermanager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Landing()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            BTUser user = await _usermanager.GetUserAsync(User);

            DashboardViewModel model = new();

            int companyId = User.Identity.GetCompanyId().Value;
            model.AllProjects = await _projectService.GetAllProjectsByCompanyAsync(companyId);

            int ticketCount = model.AllProjects.SelectMany(p => p.Tickets).ToList().Count;
            int uaTicketCount = 0;
            foreach (var item in model.AllProjects.SelectMany(p => p.Tickets).ToList())
            {
                if (string.IsNullOrEmpty(item.DeveloperUserId))
                {
                    uaTicketCount++;
                }
            }

            model.MyProjects = await _projectService.GetUserProjectsAsync(user.Id);

            if (User.IsInRole(Roles.Admin.ToString()))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetUserProjectsAsync(user.Id), "Id", "Name");
            }

            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name");

            ViewData["ticketCount"] = ticketCount;
            ViewData["unassignedTickets"] = uaTicketCount;

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}