namespace RedisFundamentals.WebApp
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var settings = config.Build();
                    config.AddAzureAppConfiguration(options =>
                    {
                        options.Connect(
                            settings["ConnectionStrings:AppConfiguration"])
                            .ConfigureRefresh(refresh =>
                            {
                                refresh.Register("Cache:Type");
                                refresh.Register("Cache:TimeToExpireInSeconds");
                                refresh.Register("Cache:TimeToRenewInSeconds");
                                refresh.Register("ConnectionStrings:Redis");
                                refresh.Register("ApplicationInsights:InstrumentationKey");
                                refresh.Register("WebApi");
                            });
                    });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
