namespace RedisFundamentals.WebApp
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using RedisFundamentals.WebApp.Cache;
    using RedisFundamentals.WebApp.Config;
    using RedisFundamentals.WebApp.Models;
    using RedisFundamentals.WebApp.Services;
    using StackExchange.Redis;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Runtime.Versioning;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            services.AddRazorPages();

            services.AddApplicationInsightsTelemetry(Configuration);
            services.Configure<ConnectionStringOptions>(Configuration.GetSection("ConnectionStrings"));

            services.AddSingleton<IConnectionMultiplexer, ConnectionMultiplexer>(c =>
                ConnectionMultiplexer.Connect(c.GetRequiredService<IOptions<ConnectionStringOptions>>().Value.Redis)
            );

            services.AddScoped<ICache, RedisCache>();
            services.AddScoped<Microsoft.Extensions.Caching.Memory.IMemoryCache, Microsoft.Extensions.Caching.Memory.MemoryCache>();

            services.AddHttpClient("API", client =>
            {
                const string clientUserAgent = "Redis Fundamentals";
                const string clientMediaType = "application/json";

                client.BaseAddress = new Uri(Configuration["WebApi"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(clientMediaType));
                client.DefaultRequestHeaders.Add("User-Agent", clientUserAgent);
            });

            services.AddScoped<IRedisAPIService>(provider => {
                int timeToExpireInSeconds = int.Parse(Configuration["Cache:TimeToExpireInSeconds"]);
                int timeToRenewInSeconds = int.Parse(Configuration["Cache:TimeToRenewInSeconds"]);
                switch (Configuration["Cache:Type"])
                {
                    case "CACHEFIRST":
                        return new RedisAPIServiceCacheFirst(
                            provider.GetRequiredService<IHttpClientFactory>(),
                            provider.GetRequiredService<TelemetryClient>(),
                            provider.GetRequiredService<ICache>(),
                            timeToExpireInSeconds);
                    case "SERVICEFIRST":
                        return new RedisAPIServiceServiceFirst(
                            provider.GetRequiredService<IHttpClientFactory>(),
                            provider.GetRequiredService<TelemetryClient>(),
                            provider.GetRequiredService<ICache>(),
                            timeToExpireInSeconds);
                    case "SLIDINGCACHE":
                        return new RedisAPIServiceSlidingCache(provider.GetRequiredService<IHttpClientFactory>(),
                            provider.GetRequiredService<TelemetryClient>(),
                            provider.GetRequiredService<ICache>(),
                            timeToExpireInSeconds,
                            timeToRenewInSeconds);
                    default:
                        return new RedisAPIServiceNoCache(
                            provider.GetRequiredService<IHttpClientFactory>(),
                            provider.GetRequiredService<TelemetryClient>());
                }
            });

            services.AddScoped<SystemInfo, SystemInfo>(c =>
                new SystemInfo(Environment.MachineName, Environment.OSVersion.VersionString, Assembly
                    .GetEntryAssembly()?
                    .GetCustomAttribute<TargetFrameworkAttribute>()?
                    .FrameworkName)
            );

            services.AddAzureAppConfiguration();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAzureAppConfiguration();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
