namespace RedisFundamentals.WebApp.Services
{
    using Microsoft.ApplicationInsights;
    using RedisFundamentals.DTO;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class RedisAPIServiceNoCache : IRedisAPIService
    {
        private readonly HttpClient client;
        private readonly TelemetryClient telemetryClient;

        public RedisAPIServiceNoCache(IHttpClientFactory clientFactory, TelemetryClient telemetryClient)
        {
            this.client = clientFactory.CreateClient("API");
            this.telemetryClient = telemetryClient;
        }

        public virtual async Task<List<Repository>> GetRepos(string organization)
        {
            try
            {
                var getReposTask = client.GetStreamAsync($"/repos/{organization}");
                return await JsonSerializer.DeserializeAsync<List<Repository>>(await getReposTask);
            }
            catch (Exception ex)
            {
                telemetryClient.TrackException(ex);
                throw;
            }
        }

        public virtual async Task<Repository> GetRepo(string repoName)
        {
            try
            {
                var getReposTask = client.GetStreamAsync($"/repo/{repoName}");
                return await JsonSerializer.DeserializeAsync<Repository>(await getReposTask);
            }
            catch (Exception ex)
            {
                telemetryClient.TrackException(ex);
                throw;
            }
        }
    }
}
