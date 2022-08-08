using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Orleans.Hosting;

namespace BlazorVanillaServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .UseOrleans(builder =>
                {
                    builder.UseLocalhostClustering();
                    builder.AddMemoryGrainStorageAsDefault();
                    builder.AddSimpleMessageStreamProvider("SMS");
                    builder.AddMemoryGrainStorage("PubSubStore");
                })
                .Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
