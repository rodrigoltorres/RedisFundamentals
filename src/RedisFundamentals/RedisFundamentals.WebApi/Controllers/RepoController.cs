namespace RedisFundamentals.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RedisFundamentals.WebApi.Services;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [ApiController]
    public class RepoController : ControllerBase
    {
        private readonly IGitHubService gitHubService;

        public RepoController(IGitHubService gitHubService)
        {
            this.gitHubService = gitHubService;
        }

        [HttpGet("repos/{organization}")]
        public async Task<IEnumerable<DTO.Repository>> GetRepos(string organization)
        {
            return await gitHubService.GetRepos(organization);
        }

        [HttpGet("repo/{repoName}")]
        public async Task<DTO.Repository> GetRepo(string repoName)
        {
            return await gitHubService.GetRepo(repoName);
        }
    }
}
