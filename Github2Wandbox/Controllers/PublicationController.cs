using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Github2Wandbox.Models;
using Github2Wandbox.ViewModels;
using System.Threading.Tasks;
using Github2Wandbox.Repository;
using Github2Wandbox.Models.Github;
using Github2Wandbox.Models.Wandbox;
using Github2Wandbox.Models.Communication;

namespace Github2Wandbox.Controllers
{
    public class PublicationController : Controller
    {
        public static string UserAgent { get; } = "Github2Wandbox";

        private readonly ILogger<HomeController> logger;
        private readonly IHttpClient httpClient;
        private readonly GithubToWandbox githubToWandbox;

        public PublicationController(ILogger<HomeController> logger,
            PublicationsContext publicationsContext)
        {
            this.logger = logger;

            httpClient = new HttpClient();
            httpClient.AddUserAgent(UserAgent);

            githubToWandbox = new GithubToWandbox(
                publicationsContext,
                new GithubDirectoryCommitChecker(httpClient),
                new GithubDirectoryScanner(httpClient),
                new WandboxPublisher(httpClient));
        }

        [HttpGet("[controller]/{" + nameof(OptionsViewModel.owner) + "}/" +
            "{" + nameof(OptionsViewModel.repository) + "}/" +
            "{*" + nameof(OptionsViewModel.main_path) + "}")]
        public async Task<IActionResult> Index(OptionsViewModel optionsViewModel)
        {
            var description = new TransformationDescription
            {
                GithubDirectoryDescription = new GithubDirectoryDescription
                {
                    Owner = optionsViewModel.owner,
                    Repository = optionsViewModel.repository,
                    MainPath = optionsViewModel.main_path
                },
                WandboxOptions = new WandboxOptions
                {
                    CompilerStandard = optionsViewModel.compiler_standard
                }
            };
            string wandboxUrl = await githubToWandbox.TransformAsync(description);
            return Redirect(wandboxUrl);
        }
    }
}
