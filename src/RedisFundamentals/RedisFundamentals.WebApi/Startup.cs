namespace RedisFundamentals.WebApi
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.ResponseCompression;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using System;
    using System.IO.Compression;
    using System.Net.Http.Headers;

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
            services.AddControllers();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo {
                    Title = "Redis API",
                    Version = "v1",
                    Description = "QuickStart with Redis",
                    Contact = new OpenApiContact {
                        Name = "Rodrigo Torres's GitHub site",
                        Url = new Uri("https://github.com/rodrigoltorres")
                    }
                }); ;
            });

            services.AddHttpClient<Services.IGitHubService, Services.GitHubService>(client =>
            {
                const string clientUserAgent = "Redis Fundamentals";
                const string clientMediaType = "application/vnd.github.v3+json";
                const string clientUrl = "https://api.github.com/";

                client.BaseAddress = new Uri(clientUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(clientMediaType));
                client.DefaultRequestHeaders.Add("User-Agent", clientUserAgent);
            });

            services.AddResponseCompression();

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();
            }

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Redis API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCompression();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
