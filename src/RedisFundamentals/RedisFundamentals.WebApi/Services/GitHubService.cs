namespace RedisFundamentals.WebApi.Services
{
    using RedisFundamentals.DTO;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class GitHubService : IGitHubService
    {
        private readonly HttpClient client;

        public GitHubService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<IEnumerable<Repository>> GetRepos(string organization)
        {
            var getReposTask = client.GetStreamAsync($"orgs/{organization}/repos");
            using Stream decompressed = new GZipStream(await getReposTask, CompressionMode.Decompress);
            return await JsonSerializer.DeserializeAsync<List<Repository>>(decompressed);
        }

        public async Task<Repository> GetRepo(string repoName)
        {
            var getReposTask = client.GetStreamAsync($"/repos/{repoName.Replace("--", "/")}");

            using Stream decompressed = new GZipStream(await getReposTask, CompressionMode.Decompress);
            return await JsonSerializer.DeserializeAsync<Repository>(decompressed);
        }
    }
}
