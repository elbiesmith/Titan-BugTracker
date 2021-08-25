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
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _roleService;

        public BTProjectService(ApplicationDbContext context, IBTRolesService roleService)
        {
            _context = context;
            _roleService = roleService;
        }

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
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ArchiveProjectAsync(Project project)
        {
            try
            {
                project.Archived = true;
                _context.Update(project);
                await _context.SaveChangesAsync();
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
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetAllProjectsByCompany(int companyId)
        {
            List<Project> result = new();
            try
            {
                result = await _context.Projects.Where(p => p.CompanyId == companyId)
                                                .Include(p => p.ProjectPriority).ToListAsync();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            List<Project> projects = new();
            List<Project> result = new();
            try
            {
                projects = await GetAllProjectsByCompany(companyId);
                result = projects.Where(p => p.ProjectPriority.Name == priorityName).ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            try
            {
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            try
            {
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            try
            {
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
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            try
            {
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            try
            {
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> IsUserOnProject(string userId, int projectId)
        {
            try
            {
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
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            try
            {
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}