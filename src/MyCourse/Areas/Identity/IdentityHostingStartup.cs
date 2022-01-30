[assembly: HostingStartup(typeof(MyCourse.Areas.Identity.IdentityHostingStartup))]
namespace MyCourse.Areas.Identity;
public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
        });
    }
}
