namespace RedisFundamentals.WebApi.Services
{
    using RedisFundamentals.DTO;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGitHubService
    {
        Task<IEnumerable<Repository>> GetRepos(string organization);
        Task<Repository> GetRepo(string repoName);
    }
}
