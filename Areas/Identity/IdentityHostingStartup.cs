using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Titan_BugTracker.Areas.Identity.IdentityHostingStartup))]

namespace Titan_BugTracker.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}