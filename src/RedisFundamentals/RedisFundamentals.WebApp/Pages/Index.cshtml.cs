namespace RedisFundamentals.WebApp.Pages
{
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using RedisFundamentals.DTO;
    using RedisFundamentals.WebApp.Models;
    using RedisFundamentals.WebApp.Services;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class IndexModel : PageModel
    {
        private readonly IRedisAPIService gitHubService;

        public SystemInfo SystemInfo;
        public List<Repository> Repos;

        public IndexModel(SystemInfo systemInfo, IRedisAPIService gitHubService)
        {
            this.SystemInfo = systemInfo;
            this.gitHubService = gitHubService;
        }

        public async Task OnGet()
        {
            TempData["MensagemFixa"] = "Teste";

            try
            {
                Repos = await gitHubService.GetRepos(GetOrnanizationName());
            }
            catch(Exception ex)
            {
                Repos = new List<Repository>();
                TempData["MensagemFixa"] = ex.Message;
            }
        }

        private string GetOrnanizationName()
        {
            string orgName = Request.Query["orgName"].ToString();
            if (string.IsNullOrEmpty(orgName)) orgName = "azurenapratica";
            return orgName;
        }
    }
}
