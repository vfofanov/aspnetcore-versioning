using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace TestSample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args);
            builder.UseStartup<Startup>();
            return builder;
        }
    }
}
