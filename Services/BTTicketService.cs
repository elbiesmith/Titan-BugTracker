﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Titan_BugTracker.Data;
using Titan_BugTracker.Models;
using Titan_BugTracker.Models.Enums;
using Titan_BugTracker.Services.Interfaces;

namespace Titan_BugTracker.Services
{
    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;
        private readonly IBTProjectService _projectService;

        public BTTicketService(ApplicationDbContext context, IBTRolesService rolesService, IBTProjectService projectService)
        {
            _context = context;
            _rolesService = rolesService;
            _projectService = projectService;
        }

        //CRUD : Create
        public async Task AddNewTicketAsync(Ticket ticket)
        {
            try
            {
                await _context.AddAsync(ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //CRUD : Delete
        public async Task ArchiveTicketAsync(Ticket ticket)
        {
            try
            {
                ticket.Archived = true;
                await UpdateTicketAsync(ticket);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            try
            {
                Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

                if (ticket != null)
                {
                    try
                    {
                        ticket.Updated = DateTimeOffset.Now;
                        ticket.DeveloperUserId = userId;
                        ticket.TicketStatusId = (await LookupTicketStatusIdAsync(BTTicketStatus.Development.ToString())).Value;
                        await UpdateTicketAsync(ticket);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            List<Ticket> tickets = new();
            try
            {
                tickets = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                 .SelectMany(t => t.Tickets)
                                                  .Where(t => t.Archived == false)
                                                  .Include(t => t.Comments)
                                                  .Include(t => t.Attachments)
                                                  .Include(t => t.History)
                                                  .Include(t => t.Notifications)
                                                  .Include(t => t.DeveloperUser)
                                                  .Include(t => t.OwnerUser)
                                                  .Include(t => t.TicketStatus)
                                                  .Include(t => t.TicketPriority)
                                                  .Include(t => t.TicketType)
                                                  .Include(p => p.Project)
                                                  .ToListAsync();

                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            List<Ticket> tickets = new();

            try
            {
                int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value;
                tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(p => p.TicketPriorityId == priorityId).ToList();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            List<Ticket> tickets = new();

            try
            {
                int statusId = (await LookupTicketStatusIdAsync(statusName)).Value;
                tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(p => p.TicketStatusId == statusId).ToList();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllDeveloperTicketsByStatusAsync(int companyId, string statusName, string userId)
        {
            List<Ticket> tickets = new();

            try
            {
                int statusId = (await LookupTicketStatusIdAsync(statusName)).Value;
                tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(p => p.TicketStatusId == statusId).Where(p => p.DeveloperUserId == userId).ToList();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            List<Ticket> tickets = new();

            try
            {
                int ticketType = (await LookupTicketTypeIdAsync(typeName)).Value;
                tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(p => p.TicketTypeId == ticketType).ToList();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
        {
            List<Ticket> tickets = new();
            try
            {
                //tickets = await _context.Tickets.Where(p => p.Project.CompanyId == companyId && p.Archived == true)
                //                                .Include(t => t.Comments)
                //                                .Include(t => t.Attachments)
                //                                .Include(t => t.History)
                //                                .Include(t => t.Notifications)
                //                                .Include(t => t.DeveloperUser)
                //                                .Include(t => t.OwnerUser)
                //                                .Include(t => t.TicketStatus)
                //                                .Include(t => t.TicketPriority)
                //                                .Include(t => t.TicketType)
                //                                .ToListAsync();

                tickets = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                 .SelectMany(t => t.Tickets)
                                                  .Where(t => t.Archived == true)
                                                  .Include(t => t.Comments)
                                                  .Include(t => t.Attachments)
                                                  .Include(t => t.History)
                                                  .Include(t => t.Notifications)
                                                  .Include(t => t.DeveloperUser)
                                                  .Include(t => t.OwnerUser)
                                                  .Include(t => t.TicketStatus)
                                                  .Include(t => t.TicketPriority)
                                                  .Include(t => t.TicketType)
                                                  .Include(p => p.Project)
                                                  .ToListAsync();

                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByPriorityAsync(companyId, priorityName)).Where(p => p.ProjectId == projectId).ToList();

                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
        {
            List<Ticket> tickets = new();
            try
            {
                tickets = (await GetTicketsByRoleAsync(role, userId, companyId)).Where(t => t.ProjectId == projectId).ToList();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByStatusAsync(companyId, statusName)).Where(p => p.ProjectId == projectId).ToList();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                tickets = (await GetAllTicketsByTypeAsync(companyId, typeName)).Where(p => p.ProjectId == projectId).ToList();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //CRUD : Read
        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            try
            {
                Ticket ticket = await _context.Tickets.Include(t => t.DeveloperUser)
                                                        .Include(t => t.OwnerUser)
                                                        .Include(t => t.Project)
                                                        .Include(t => t.TicketPriority)
                                                        .Include(t => t.TicketStatus)
                                                        .Include(t => t.TicketType)
                                                        .Include(t => t.Comments).ThenInclude(t => t.User)
                                                        .Include(t => t.Attachments).ThenInclude(t => t.User)
                                                        .Include(t => t.History).ThenInclude(t => t.User)
                                                        .FirstOrDefaultAsync(p => p.Id == ticketId);

                return ticket;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BTUser> GetTicketDeveloperAsync(int ticketId, int companyId)
        {
            BTUser developer = new();

            try
            {
                Ticket ticket = (await GetAllTicketsByCompanyAsync(companyId)).FirstOrDefault(t => t.Id == ticketId);

                if (ticket.DeveloperUserId != null)
                {
                    developer = ticket.DeveloperUser;
                }

                return developer;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
        {
            List<Ticket> tickets = new();
            try
            {
                if (role == Roles.Admin.ToString())
                {
                    tickets = await GetAllTicketsByCompanyAsync(companyId);
                }
                else if (role == Roles.Developer.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => t.DeveloperUserId == userId).ToList();
                }
                else if (role == Roles.Submitter.ToString())
                {
                    tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => t.OwnerUserId == userId).ToList();
                }
                else if (role == Roles.ProjectManager.ToString())
                {
                    tickets = await GetTicketsByUserIdAsync(userId, companyId);
                }

                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            List<Ticket> tickets = new();
            try
            {
                BTUser btUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (await _rolesService.IsUserInRoleAsync(btUser, Roles.Admin.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyAsync(companyId)).SelectMany(p => p.Tickets).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser, Roles.Developer.ToString()))
                {
                    List<Ticket> devTickets = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                        .SelectMany(p => p.Tickets).Where(t => t.DeveloperUserId == userId).ToList();

                    List<Ticket> subTicket = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                        .SelectMany(p => p.Tickets).Where(t => t.OwnerUserId == userId).ToList();

                    tickets = devTickets.Concat(subTicket).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser, Roles.Submitter.ToString()))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyAsync(companyId))
                        .SelectMany(t => t.Tickets).Where(t => t.OwnerUserId == userId).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser, Roles.ProjectManager.ToString()))
                {
                    tickets = (await _projectService.GetUserProjectsAsync(userId)).SelectMany(t => t.Tickets).ToList();
                }

                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            try
            {
                int priorityId = (await _context.TicketPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;

                return priorityId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            try
            {
                TicketStatus statusId = await _context.TicketStatuses.FirstOrDefaultAsync(p => p.Name == statusName);

                return statusId.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            try
            {
                TicketType typeId = await _context.TicketTypes.FirstOrDefaultAsync(p => p.Name == typeName);

                return typeId.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // CRUD: Update
        public async Task UpdateTicketAsync(Ticket ticket)
        {
            try
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId)
        {
            try
            {
                Ticket ticket = await _context.Tickets.Include(t => t.DeveloperUser)
                                                        .Include(t => t.OwnerUser)
                                                        .Include(t => t.DeveloperUser)
                                                        .Include(t => t.Project)
                                                        .Include(t => t.TicketPriority)
                                                        .Include(t => t.TicketStatus)
                                                        .Include(t => t.TicketType)
                                                        .Include(t => t.Comments).ThenInclude(t => t.User)
                                                        .Include(t => t.Attachments).ThenInclude(t => t.User)
                                                        .Include(t => t.History).ThenInclude(t => t.User)
                                                        .AsNoTracking().FirstOrDefaultAsync(p => p.Id == ticketId);

                return ticket;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}