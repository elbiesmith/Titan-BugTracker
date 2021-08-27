﻿using Microsoft.AspNetCore.Http;
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

        public Task<bool> AcceptInviteAsync(Guid? token, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyInviteAsync(Guid token, string email)
        {
            throw new NotImplementedException();
        }

        public Task<Invite> GetInviteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Invite> GetInviteAsync(Guid token, string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateInviteCodeAsync(Guid? token)
        {
            throw new NotImplementedException();
        }
    }
}