namespace RedisFundamentals.WebApp.Services
{
    using RedisFundamentals.DTO;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRedisAPIService
    {
        Task<Repository> GetRepo(string repoName);
        Task<List<Repository>> GetRepos(string organization);
    }
}