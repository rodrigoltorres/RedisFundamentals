using StackExchange.Redis;

namespace RedisFundamentals.WebApp.Cache
{
    public interface IRedisService
    {
        IDatabase GetDatabase();
    }
}