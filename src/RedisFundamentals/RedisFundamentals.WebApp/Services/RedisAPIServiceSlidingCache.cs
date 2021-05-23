namespace RedisFundamentals.WebApp.Services
{
    using Microsoft.ApplicationInsights;
    using RedisFundamentals.DTO;
    using RedisFundamentals.WebApp.Cache;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class RedisAPIServiceSlidingCache : RedisAPIServiceNoCache
    {
        private readonly ICache cache;
        private readonly TimeSpan cacheTime;
        private readonly TimeSpan cacheTimeToRenew;

        public RedisAPIServiceSlidingCache(
            IHttpClientFactory clientFactory,
            TelemetryClient telemetryClient,
            ICache cache,
            int cacheTimeInSeconds,
            int cacheTimeToRenewInSeconds)
            : base (clientFactory, telemetryClient)
        {
            this.cache = cache;
            cacheTime = TimeSpan.FromSeconds(cacheTimeInSeconds);
            cacheTimeToRenew = TimeSpan.FromSeconds(cacheTimeToRenewInSeconds);
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
            else
            {
                var timeRemaining = cache.KeyTimeToLive(redisKey);
                if (timeRemaining.HasValue && timeRemaining < cacheTimeToRenew)
                {
                    try
                    {
                        valueFromCache = await base.GetRepo(repoName);
                        cache.Add(redisKey, valueFromCache, cacheTime);
                    }
                    catch { }
                }
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
            else
            {
                var timeRemaining = cache.KeyTimeToLive(redisKey);
                if (timeRemaining.HasValue && timeRemaining < cacheTimeToRenew)
                {
                    try
                    {
                        valueFromCache = await base.GetRepos(organization);
                        cache.Add(redisKey, valueFromCache, cacheTime);
                    }
                    catch { }
                }
            }
            return valueFromCache;
        }

        private string BuildRepoKey(string repoName)
            => $"repository:{repoName}";

        private string BuildOrganizationKey(string organization)
            => $"repository:{organization}";
    }
}
