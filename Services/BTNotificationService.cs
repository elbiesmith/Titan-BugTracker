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
    public class BTNotificationService : IBTNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IBTRolesService _rolesService;

        public BTNotificationService(ApplicationDbContext context, IEmailSender emailSender, IBTRolesService rolesService)
        {
            _context = context;
            _emailSender = emailSender;
            _rolesService = rolesService;
        }

        public Task AddNotificationAsync(Notification notification)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Notification>> GetReceivedNotificationsAsync(string userId)
        {
            List<Notification> notifications = new();
            try
            {
                notifications = await _context.Notifications.Where(n => n.RecipientId == userId).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return notifications;
        }

        public async Task<List<Notification>> GetSentNotificationsAsync(string userId)
        {
            List<Notification> notifications = new();
            try
            {
                notifications = await _context.Notifications.Where(n => n.SenderId == userId).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return notifications;
        }

        public Task<bool> SendEmailNotificationAsync(Notification notification, string emailSubject)
        {
            throw new NotImplementedException();
        }

        public Task SendEmailNotificationsByRoleAsync(Notification notification, int companyId, string role)
        {
            throw new NotImplementedException();
        }

        public Task SendMembersEmailNotificationsAsync(Notification notification, List<BTUser> members)
        {
            throw new NotImplementedException();
        }
    }
}