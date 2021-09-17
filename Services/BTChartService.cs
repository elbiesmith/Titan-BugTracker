using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Titan_BugTracker.Models;
using Titan_BugTracker.Models.ViewModels;
using Titan_BugTracker.Services.Interfaces;
using Titan_BugTracker.Models.Enums;

namespace Titan_BugTracker.Services
{
    public class BTChartService : IBTChartService
    {
        private readonly IBTProjectService _projectService;
        private readonly IBTTicketService _ticketService;

        public BTChartService(IBTTicketService ticketService, IBTProjectService projectService)
        {
            _ticketService = ticketService;
            _projectService = projectService;
        }

        public async Task<ChartViewModel> GetAdminLineChart(int companyId)
        {
            ChartViewModel model = new();
            model.Projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            model.Tickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);
            int devs = 0;
            int tests = 0;

            foreach (Project project in model.Projects)
            {
                model.Labels.Add(project.Name);
                model.AllTickets.Add(project.Tickets.Count);
                foreach (Ticket ticket in project.Tickets)
                {
                    devs = 0;
                    tests = 0;
                    if (ticket.TicketStatus.Name == BTTicketStatus.Development.ToString())
                    {
                        devs++;
                    }
                    if (ticket.TicketStatus.Name == BTTicketStatus.Testing.ToString())
                    {
                        tests++;
                    }
                }
                model.DevelopmentTickets.Add(devs);
                model.TestingTickets.Add(tests);
            }

            return (model);
        }
    }
}