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
    public class BTTicketHistoryService : IBTTicketHistoryService
    {
        private readonly ApplicationDbContext _context;

        public BTTicketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId)
        {
            try
            {
                List<Project> projects = (await _context.Companies
                                                        .Include(c => c.Projects)
                                                          .ThenInclude(p => p.Tickets)
                                                            .ThenInclude(t => t.History)
                                                              .ThenInclude(h => h.User)
                                                        .FirstOrDefaultAsync(c => c.Id == companyId))
                                                        .Projects.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId)
        {
            List<TicketHistory> histories = new();
            try
            {
                Project project = await _context.Projects.Include(t => t.Tickets).ThenInclude(t => t.History).FirstOrDefaultAsync(p => p.CompanyId == companyId);
                histories = project.Tickets.SelectMany(h => h.History).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            return histories;
        }
    }
}