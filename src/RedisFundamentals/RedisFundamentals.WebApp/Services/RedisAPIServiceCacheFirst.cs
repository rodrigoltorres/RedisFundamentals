namespace RedisFundamentals.WebApp.Services
{
    using Microsoft.ApplicationInsights;
    using RedisFundamentals.DTO;
    using RedisFundamentals.WebApp.Cache;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class RedisAPIServiceCacheFirst : RedisAPIServiceNoCache
    {
        private readonly ICache cache;
        private readonly TimeSpan cacheTime;

        public RedisAPIServiceCacheFirst(
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
            var valueFromCache = cache.Get<Repository>(redisKey);
            if (valueFromCache is null)
            {
                valueFromCache = await base.GetRepo(repoName);
                cache.Add(redisKey, valueFromCache, cacheTime);
            }
            return valueFromCache;
        }

        public override async Task<List<Repository>> GetRepos(string organization)
        {
            var redisKey = BuildOrganizationKey(organization);
            var valueFromCache = cache.Get<List<Repository>>(redisKey);
            if (valueFromCache is null)
            {
                valueFromCache = await base.GetRepos(organization);
                cache.Add(redisKey, valueFromCache, cacheTime);
            }
            return valueFromCache;
        }

        private string BuildRepoKey(string repoName)
            => $"repository:{repoName}";

        private string BuildOrganizationKey(string organization)
            => $"repository:{organization}";
    }
}
