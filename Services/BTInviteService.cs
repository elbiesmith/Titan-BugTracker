﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
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
    public class BTInviteService : IBTInviteService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IBTProjectService _projectService;

        public Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task AddNewInviteAsync(Invite invite)
        {
            await _context.AddAsync(invite);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                bool result = await _context.Invites.Where(i => i.CompanyId == companyId)
                                                    .AnyAsync(i => i.CompanyToken == token && i.InviteeEmail == email);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Invite> GetInviteAsync(int inviteId, int companyId)
        {
            try
            {
                Invite invite = await _context.Invites.Where(i => i.CompanyId == companyId)
                                               .Include(i => i.Company)
                                               .Include(i => i.Project)
                                               .Include(i => i.Invitor)
                                               .FirstOrDefaultAsync(i => i.Id == inviteId);

                return invite;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Invite> GetInviteAsync(Guid token, string email, int companyId)
        {
            try
            {
                Invite invite = await _context.Invites.Where(i => i.CompanyId == companyId)
                                               .Include(i => i.Company)
                                               .Include(i => i.Project)
                                               .Include(i => i.Invitor)
                                               .FirstOrDefaultAsync(i => i.CompanyToken == token && i.InviteeEmail == email);

                return invite;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<bool> ValidateInviteCodeAsync(Guid? token)
        {
            throw new NotImplementedException();
        }
    }
}