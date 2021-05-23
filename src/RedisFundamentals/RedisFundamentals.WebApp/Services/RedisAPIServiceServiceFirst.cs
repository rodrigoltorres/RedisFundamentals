namespace RedisFundamentals.WebApp.Services
{
    using Microsoft.ApplicationInsights;
    using RedisFundamentals.DTO;
    using RedisFundamentals.WebApp.Cache;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class RedisAPIServiceServiceFirst : RedisAPIServiceNoCache
    {
        private readonly ICache cache;
        private readonly TimeSpan cacheTime;

        public RedisAPIServiceServiceFirst(
            IHttpClientFactory clientFactory,
            TelemetryClient telemetryClient,
            ICache cache,
            int cacheTimeInSeconds)
            : base (clientFactory, telemetryClient)
        {
            this.cache = cache;
            cacheTime = TimeSpan.FromSeconds(cacheTimeInSeconds);
        }

        public override async Task<Repository> GetRepo(string repoName)
        {
            var redisKey = BuildRepoKey(repoName);
            try
            {
                var valueFromService = await base.GetRepo(repoName);
                cache.Add(redisKey, valueFromService, cacheTime);
                return valueFromService;
            }
            catch
            {
                return cache.Get<Repository>(redisKey);
            }
        }

        public override async Task<List<Repository>> GetRepos(string organization)
        {
            var redisKey = BuildOrganizationKey(organization);
            try
            {
                var valueFromService = await base.GetRepos(organization);
                cache.Add(redisKey, valueFromService, cacheTime);
                return valueFromService;
            }
            catch
            {
                return cache.Get<List<Repository>>(redisKey);
            }
        }

        private string BuildRepoKey(string repoName)
            => $"repository:{repoName}";

        private string BuildOrganizationKey(string organization)
            => $"repository:{organization}";
    }
}
