using System.Collections.Generic;
using System.Threading.Tasks;
using Titan_BugTracker.Models;

namespace Titan_BugTracker.Services.Interfaces
{
    public interface IBTCompanyInfoService
    {
        public Task<Company> GetCompanyInfoByIdAsync(int? companyId);

        public Task<List<BTUser>> GetAllMembersAsync(int companyId);

        public Task<List<Project>> GetAllProjectsAsync(int companyId);

        public Task<List<Ticket>> GetAllTicketsAsync(int companyId);

        public Task<List<Project>> GetArchivedProjectsAsync(int companyId);
    }
}