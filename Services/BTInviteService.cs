﻿using Microsoft.EntityFrameworkCore;
using System;
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

        public BTInviteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
        {
            try
            {
                Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);

                if (invite != null)
                {
                    try
                    {
                        invite.IsValid = false;
                        invite.InviteeId = userId;
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
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

        public async Task<bool> ValidateInviteCodeAsync(Guid? token)
        {
            try
            {
                if (token == null)
                {
                    return false;
                }

                bool result = false;
                Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);

                if (invite != null)
                {
                    // Determine Invite Date
                    DateTime inviteDate = invite.InviteDate.DateTime;

                    // Custom Validation of invite based on the date it was issued.
                    // in this case, invites are valid for 7 days.
                    bool validDate = (DateTime.Now - inviteDate).TotalDays <= 7;

                    if (validDate)
                    {
                        result = invite.IsValid;
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}