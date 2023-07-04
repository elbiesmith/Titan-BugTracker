using System.Threading.Tasks;
using Titan_BugTracker.Models.ViewModels;

namespace Titan_BugTracker.Services.Interfaces
{
    public interface IBTChartService
    {
        public Task<ChartViewModel> GetAdminLineChart(int companyId);
    }
}