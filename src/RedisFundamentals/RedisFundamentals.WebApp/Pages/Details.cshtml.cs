namespace RedisFundamentals.WebApp.Pages
{
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using RedisFundamentals.DTO;
    using RedisFundamentals.WebApp.Models;
    using RedisFundamentals.WebApp.Services;
    using System;
    using System.Threading.Tasks;

    public class DetailsModel : PageModel
    {
        private readonly IRedisAPIService gitHubService;

        public SystemInfo SystemInfo;
        public Repository Repo;

        public DetailsModel(SystemInfo systemInfo, IRedisAPIService gitHubService)
        {
            this.SystemInfo = systemInfo;
            this.gitHubService = gitHubService;
        }

        public async Task OnGet(string repoName)
        {
            TempData["MensagemFixa"] = "Teste";
            try
            {
                Repo = await gitHubService.GetRepo(repoName);
            }
            catch (Exception ex)
            {
                TempData["MensagemFixa"] = ex.Message;
            }
            
        }
    }
}
