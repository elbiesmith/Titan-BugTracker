using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Titan_BugTracker.Data;
using Titan_BugTracker.Extensions;
using Titan_BugTracker.Models;
using Titan_BugTracker.Models.Enums;
using Titan_BugTracker.Models.ViewModels;
using Titan_BugTracker.Services.Interfaces;

namespace Titan_BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTTicketService _ticketService;
        private readonly IBTTicketHistoryService _historyService;
        private readonly IBTNotificationService _notificationService;
        private readonly IBTRolesService _rolesService;

        public TicketsController(ApplicationDbContext context, IBTProjectService projectService, UserManager<BTUser> userManager,
            IBTTicketService ticketService, IBTTicketHistoryService historyService, IBTNotificationService notificationService, IBTRolesService rolesService)
        {
            _context = context;
            _projectService = projectService;
            _userManager = userManager;
            _ticketService = ticketService;
            _historyService = historyService;
            _notificationService = notificationService;
            _rolesService = rolesService;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Ticket> allTickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);
            return View(allTickets);
        }

        public async Task<IActionResult> AllTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Ticket> allTickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);
            return View(allTickets);
        }

        public async Task<IActionResult> ArchiveTicket(int id)
        {
            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);
            await _ticketService.ArchiveTicketAsync(ticket);

            return RedirectToAction("Dashboard", "Home");
        }

        public async Task<IActionResult> MyTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            string userId = _userManager.GetUserId(User);

            List<Ticket> myTickets = await _ticketService.GetTicketsByUserIdAsync(userId, companyId);
            return View(myTickets);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _ticketService.GetTicketByIdAsync((int)id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        public async Task<IActionResult> NotificationDetails(int? id, int notificationId)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _notificationService.MarkNotificationAsRead(notificationId);

            var ticket = await _ticketService.GetTicketByIdAsync((int)id);
            if (ticket == null)
            {
                return NotFound();
            }

            return RedirectToAction("Details", "Tickets", new { id = id.Value });
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            BTUser user = await _userManager.GetUserAsync(User);
            int companyId = User.Identity.GetCompanyId().Value;

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
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);

                ticket.Created = DateTimeOffset.Now;
                ticket.OwnerUserId = btUser.Id;

                ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync(BTTicketStatus.New.ToString())).Value;

                await _ticketService.AddNewTicketAsync(ticket);

                //TODO: Add to History

                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                await _historyService.AddHistoryAsync(null, newTicket, btUser.Id);

                //TODO: Send Notification
                BTUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);
                int companyId = User.Identity.GetCompanyId().Value;
                BTUser admin = (await _rolesService.GetUsersInRoleAsync(Roles.Admin.ToString(), companyId)).FirstOrDefault();

                Notification notification = new()
                {
                    TicketId = ticket.Id,
                    Title = "New Ticket Created",
                    Message = $"New Ticket: {ticket.Title}, was created by {btUser.FullName}.",
                    Created = DateTimeOffset.Now,
                    SenderId = btUser.Id,
                    RecipientId = projectManager?.Id
                };

                if (projectManager != null)
                {
                    await _notificationService.AddNotificationAsync(notification);
                    await _notificationService.SendEmailNotificationAsync(notification, "New Ticket Added");
                }
                else
                {
                    notification.RecipientId = admin.Id;
                    await _notificationService.AddNotificationAsync(notification);
                    await _notificationService.SendEmailNotificationsByRoleAsync(notification, companyId, Roles.Admin.ToString());
                }

                return RedirectToAction("Details", "Projects", new { id = ticket.ProjectId });
            }

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,TicketTypeId,TicketStatusId,TicketPriorityId, ProjectId, Created, OwnerUserId, DeveloperUserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);
                Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                try
                {
                    ticket.Updated = DateTimeOffset.Now;
                    await _ticketService.UpdateTicketAsync(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                //TODO: Add to History
                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);
                await _historyService.AddHistoryAsync(oldTicket, newTicket, btUser.Id);

                //TODO: Send Notification
                BTUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);
                int companyId = User.Identity.GetCompanyId().Value;
                BTUser admin = (await _rolesService.GetUsersInRoleAsync(Roles.Admin.ToString(), companyId)).FirstOrDefault();

                Notification notification = new()
                {
                    TicketId = ticket.Id,
                    Title = "Ticket Updated",
                    Message = $"Ticket: {ticket.Title}, was updated by {btUser.FullName}.",
                    Created = DateTimeOffset.Now,
                    SenderId = btUser.Id,
                    RecipientId = projectManager?.Id
                };

                if (projectManager != null)
                {
                    await _notificationService.AddNotificationAsync(notification);
                    await _notificationService.SendEmailNotificationAsync(notification, notification.Message);
                }
                else
                {
                    notification.RecipientId = admin.Id;
                    await _notificationService.AddNotificationAsync(notification);
                    await _notificationService.SendEmailNotificationsByRoleAsync(notification, companyId, Roles.Admin.ToString());
                }
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }
            //ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.DeveloperUserId);
            //ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerUserId);
            //ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", ticket.ProjectId);
            //ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id", ticket.TicketPriorityId);
            //ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticket.TicketStatusId);
            //ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id", ticket.TicketTypeId);
            return View(ticket);
        }

        [HttpGet]
        public async Task<IActionResult> AssignDeveloper(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            AssignDeveloperViewModel model = new();
            model.Ticket = await _ticketService.GetTicketByIdAsync(id);
            model.Developers = new SelectList(await _projectService.GetProjectMembersByRoleAsync(model.Ticket.ProjectId, Roles.Developer.ToString()),
               "Id", "FullName");
            model.Project = await _projectService.GetProjectByIdAsync(model.Ticket.ProjectId, companyId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignDeveloper(AssignDeveloperViewModel model)
        {
            //TODO: Add to history && send notifications
            //old ticket and user
            BTUser btUser = await _userManager.GetUserAsync(User);
            Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);
            if (model.DeveloperId != null)
            {
                try
                {
                    await _ticketService.AssignTicketAsync(model.Ticket.Id, model.DeveloperId);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            // new ticket
            //addhistory
            Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);
            await _historyService.AddHistoryAsync(oldTicket, newTicket, btUser.Id);
            //notification
            BTUser projectManager = await _projectService.GetProjectManagerAsync(model.Ticket.ProjectId);
            int companyId = User.Identity.GetCompanyId().Value;
            BTUser admin = (await _rolesService.GetUsersInRoleAsync(Roles.Admin.ToString(), companyId)).FirstOrDefault();
            BTUser dev = (await _rolesService.GetUsersInRoleAsync(Roles.Developer.ToString(), companyId)).FirstOrDefault();

            Notification notification = new()
            {
                TicketId = model.Ticket.Id,
                Title = "Developer Added",
                Message = $"Ticket: {model.Ticket.Title}, was updated by {btUser.FullName}. {dev.FullName} was added as the Developer.",
                Created = DateTimeOffset.Now,
                SenderId = btUser.Id,
                RecipientId = projectManager?.Id
            };

            if (projectManager != null)
            {
                await _notificationService.AddNotificationAsync(notification);
                await _notificationService.SendEmailNotificationAsync(notification, notification.Message);
            }
            else
            {
                notification.RecipientId = admin.Id;
                await _notificationService.AddNotificationAsync(notification);
                await _notificationService.SendEmailNotificationsByRoleAsync(notification, companyId, Roles.Admin.ToString());
            }
            return RedirectToAction("AllTickets");
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TicketExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            return (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Any(t => t.Id == id);

            return _context.Tickets.Any(e => e.Id == id);
        }

        public IActionResult ShowFile(int id)
        {
            TicketAttachment ticketAttachment = _context.TicketAttachments.Find(id);
            string fileName = ticketAttachment.FileName;
            byte[] fileData = ticketAttachment.FileData;
            string ext = Path.GetExtension(fileName).Replace(".", "");

            Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
            return File(fileData, $"application/{ext}");
        }
    }
}