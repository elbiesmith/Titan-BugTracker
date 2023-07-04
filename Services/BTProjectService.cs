using Microsoft.EntityFrameworkCore;
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
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _roleService;

        public BTProjectService(ApplicationDbContext context, IBTRolesService roleService)
        {
            _context = context;
            _roleService = roleService;
        }

        // Crud : Create
        public async Task AddNewProjectAsync(Project project)
        {
            try
            {
                await _context.AddAsync(project);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            try
            {
                BTUser currentPM = await GetProjectManagerAsync(projectId);

                if (currentPM != null)
                {
                    try
                    {
                        await RemoveProjectManagerAsync(projectId);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                try
                {
                    await AddUserToProjectAsync(userId, projectId);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null)
                {
                    if (!await IsUserOnProject(userId, projectId))
                    {
                        try
                        {
                            Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                            project.Members.Add(user);
                            await UpdateProjectAsync(project);
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

        // Crud : Delete
        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {
                project.Archived = true;
                await UpdateProjectAsync(project);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            try
            {
                List<BTUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
                List<BTUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
                List<BTUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

                List<BTUser> members = developers.Concat(submitters).Concat(admins).ToList();

                return members;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetAllProjectsByCompanyAsync(int companyId)
        {
            List<Project> projects = new();
            try
            {
                projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == false)
                                                .Include(p => p.Members)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.Comments)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.Attachments)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.History)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.Notifications)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.DeveloperUser)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.OwnerUser)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketStatus)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketPriority)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketType)
                                                .Include(p => p.ProjectPriority)
                                                .ToListAsync();
                return projects;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> projects = new();

            try
            {
                projects = await GetAllProjectsByCompanyAsync(companyId);
                int priorityId = await LookupProjectPriorityId(priorityName);

                return projects.Where(p => p.Id == priorityId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            List<Project> projects = new();
            try
            {
                projects = await _context.Projects.Where(p => p.CompanyId == companyId && p.Archived == true)
                                                .Include(p => p.Members)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.Comments)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.Attachments)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.History)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.Notifications)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.DeveloperUser)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.OwnerUser)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketStatus)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketPriority)
                                                .Include(p => p.Tickets)
                                                    .ThenInclude(t => t.TicketType)
                                                .Include(p => p.ProjectPriority)
                                                .ToListAsync();
                return projects;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Obsolete]
        public async Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            try
            {
                return await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Crud : Read
        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            try
            {
                Project project = await _context.Projects.Include(p => p.Tickets)
                                                 .ThenInclude(t => t.TicketPriority)
                                                .Include(p => p.Tickets)
                                                  .ThenInclude(p => p.History).ThenInclude(p => p.User)
                                                  .Include(p => p.Tickets)
                                                  .ThenInclude(p => p.Comments).ThenInclude(p => p.User)
                                                  .Include(p => p.Tickets)
                                                  .ThenInclude(p => p.TicketStatus)
                                                .Include(p => p.Members)
                                                .Include(p => p.ProjectPriority)
                                                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

                return project;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BTUser> GetProjectManagerAsync(int projectId)
        {
            try
            {
                Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
                if (project != null)
                {
                    foreach (BTUser member in project.Members)
                    {
                        if (await _roleService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                        {
                            return member;
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            try
            {
                Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
                List<BTUser> members = new();

                foreach (BTUser user in project.Members)
                {
                    if (await _roleService.IsUserInRoleAsync(user, role))
                    {
                        members.Add(user);
                    }
                }
                return members;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Obsolete]
        public async Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            return await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users.Include(u => u.Projects)
                                                                    .ThenInclude(p => p.Company)
                                                                   .Include(u => u.Projects)
                                                                    .ThenInclude(p => p.Members)
                                                                   .Include(u => u.Projects)
                                                                    .ThenInclude(p => p.Tickets)
                                                                   .Include(u => u.Projects)
                                                                    .ThenInclude(t => t.Tickets)
                                                                     .ThenInclude(t => t.DeveloperUser)
                                                                   .Include(u => u.Projects)
                                                                    .ThenInclude(t => t.Tickets)
                                                                     .ThenInclude(t => t.OwnerUser)
                                                                   .Include(u => u.Projects)
                                                                    .ThenInclude(t => t.Tickets)
                                                                     .ThenInclude(t => t.TicketPriority)
                                                                   .Include(u => u.Projects)
                                                                    .ThenInclude(t => t.Tickets)
                                                                     .ThenInclude(t => t.TicketStatus)
                                                                   .Include(u => u.Projects)
                                                                    .ThenInclude(t => t.Tickets)
                                                                     .ThenInclude(t => t.TicketType)
                                                                   .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();

                return userProjects;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** Error *** - Getting user projects list. --> {ex.Message}");
                throw;
            }
        }

        public async Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            try
            {
                List<BTUser> users = await _context.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToListAsync();
                return users.Where(u => u.CompanyId == companyId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetUnassignedProjectsAsync(int companyId)
        {
            List<Project> projects = new();
            List<Project> result = new();
            try
            {
                projects = await _context.Projects.Where(p => p.CompanyId == companyId).ToListAsync();

                foreach (Project proj in projects)
                {
                    if ((await GetProjectMembersByRoleAsync(proj.Id, Roles.ProjectManager.ToString())).Count == 0)
                    {
                        result.Add(proj);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public async Task<bool> IsUserOnProject(string userId, int projectId)
        {
            try
            {
                Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);
                bool result = false;

                if (project != null)
                {
                    result = project.Members.Any(m => m.Id == userId);
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> LookupProjectPriorityId(string priorityName)
        {
            try
            {
                int priorityId = (await _context.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;

                return priorityId;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            try
            {
                Project project = await _context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

                try
                {
                    foreach (BTUser member in project.Members)
                    {
                        if (await _roleService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                        {
                            await RemoveUserFromProjectAsync(member.Id, projectId);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                try
                {
                    if (await IsUserOnProject(userId, projectId))
                    {
                        project.Members.Remove(user);
                        await UpdateProjectAsync(project);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                List<BTUser> members = await GetProjectMembersByRoleAsync(projectId, role);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                foreach (BTUser user in members)
                {
                    try
                    {
                        project.Members.Remove(user);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                await UpdateProjectAsync(project);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Crud : Update
        public async Task UpdateProjectAsync(Project project)
        {
            try
            {
                _context.Update(project);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}