namespace RedisFundamentals.WebApp.Cache
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using StackExchange.Redis;
    using System;

    public class RedisCache : ICache
    {
        private static JsonSerializerSettings jsonSettings =
            new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.All,
                ContractResolver = new CamelCasePropertyNamesContractResolver {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

        private readonly IDatabase redis;

        public RedisCache(IConnectionMultiplexer connectionMultiplexer)
            => this.redis = connectionMultiplexer.GetDatabase();

        public bool Add(string key, object obj, TimeSpan? expiry = null)
            => redis.StringSet(key, JsonConvert.SerializeObject(obj, jsonSettings), expiry);

        public T Get<T>(string key)
        {
            var objSerialized = redis.StringGet(key);
            if (objSerialized.IsNull) return default;
            return JsonConvert.DeserializeObject<T>(objSerialized, jsonSettings);
        }

        public bool Delete(string key)
            => redis.KeyDelete(key);

        public TimeSpan? KeyTimeToLive(string key)
            => redis.KeyTimeToLive(key);
    }
}
