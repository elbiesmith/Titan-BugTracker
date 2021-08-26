using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Titan_BugTracker.Data;
using Titan_BugTracker.Models;
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
                await UpdateTicketAsync(ticket);
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

        public Task AssignTicketAsync(int ticketId, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            List<Ticket> tickets = new();
            try
            {
                tickets = await _context.Tickets.Where(p => p.Project.CompanyId == companyId && p.Archived == false)
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
                                                .Include(p => p.ProjectId)
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
                //probably need a null check in this method
                tickets = await GetAllTicketsByCompanyAsync(companyId);
                int priorityId = await LookupTicketPriorityIdAsync(priorityName);

                return tickets.Where(p => p.Id == priorityId).ToList();
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
                //probably need a null check in this method
                tickets = await GetAllTicketsByCompanyAsync(companyId);
                int statusId = await LookupTicketPriorityIdAsync(statusName);

                return tickets.Where(p => p.Id == statusId).ToList();
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
                //probably need a null check in this method
                tickets = await GetAllTicketsByCompanyAsync(companyId);
                int typeId = await LookupTicketPriorityIdAsync(typeName);

                return tickets.Where(p => p.Id == typeId).ToList();
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
                tickets = await _context.Tickets.Where(p => p.Project.CompanyId == companyId && p.Archived == true)
                                                .Include(t => t.Comments)
                                                .Include(t => t.Attachments)
                                                .Include(t => t.History)
                                                .Include(t => t.Notifications)
                                                .Include(t => t.DeveloperUser)
                                                .Include(t => t.OwnerUser)
                                                .Include(t => t.TicketStatus)
                                                .Include(t => t.TicketPriority)
                                                .Include(t => t.TicketType)
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
                //probably need a null check in this method
                tickets = await GetAllTicketsByCompanyAsync(companyId);
                int priorityId = (int)await LookupTicketPriorityIdAsync(priorityName);

                return tickets.Where(p => p.Id == priorityId && p.ProjectId == projectId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {
            List<Ticket> tickets = new();

            try
            {
                //probably need a null check in this method
                tickets = await GetAllTicketsByCompanyAsync(companyId);
                int priorityId = (int)await LookupTicketPriorityIdAsync(statusName);

                return tickets.Where(p => p.Id == priorityId && p.ProjectId == projectId).ToList();
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
                //probably need a null check in this method
                tickets = await GetAllTicketsByCompanyAsync(companyId);
                int priorityId = (int)await LookupTicketPriorityIdAsync(typeName);

                return tickets.Where(p => p.Id == priorityId && p.ProjectId == projectId).ToList();
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
                Ticket ticket = await _context.Tickets
                                                .Include(t => t.Comments)
                                                .Include(t => t.Attachments)
                                                .Include(t => t.History)
                                                .Include(t => t.Notifications)
                                                .Include(t => t.DeveloperUser)
                                                .Include(t => t.OwnerUser)
                                                .Include(t => t.TicketStatus)
                                                .Include(t => t.TicketPriority)
                                                .Include(t => t.TicketType)
                                                .FirstOrDefaultAsync(p => p.Id == ticketId);

                return ticket;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<BTUser> GetTicketDeveloperAsync(int ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            throw new NotImplementedException();
        }

        //next three returned nullable ints
        public async Task<int> LookupTicketPriorityIdAsync(string priorityName)
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

        public async Task<int> LookupTicketStatusIdAsync(string statusName)
        {
            try
            {
                int statusId = (await _context.TicketStatuses.FirstOrDefaultAsync(p => p.Name == statusName)).Id;

                return statusId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> LookupTicketTypeIdAsync(string typeName)
        {
            try
            {
                int typeId = (await _context.TicketTypes.FirstOrDefaultAsync(p => p.Name == typeName)).Id;

                return typeId;
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
    }
}