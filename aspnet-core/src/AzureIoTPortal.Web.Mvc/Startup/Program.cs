using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace AzureIoTPortal.Web.Startup
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:5000")
               .UseStartup<Startup>()
               .Build();
        }
    }
}
