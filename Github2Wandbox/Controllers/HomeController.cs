using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Github2Wandbox.Models;
using Github2Wandbox.ViewModels;

namespace Github2Wandbox.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GithubToWandbox githubToWandbox;
        private readonly PublishUrlGenerator publishUrlGenerator;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            githubToWandbox = new GithubToWandbox(new GithubDirectoryScanner(), new WandboxPublisher());
            publishUrlGenerator = new PublishUrlGenerator();
        }

        public IActionResult Index()
        {
            return View(new OptionsViewModel());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Generate(OptionsViewModel optionsViewModel)
        {
            string url = publishUrlGenerator.Generate(optionsViewModel);
            string baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/";
            return View(new GenerateViewModel{ Url =  baseUrl + url});
        }

        [HttpGet("Publish/{" + nameof(OptionsViewModel.owner) + "}/" +
            "{" + nameof(OptionsViewModel.repository) + "}/" +
            "{*" + nameof(OptionsViewModel.main_path) + "}")]
        public IActionResult Publish(OptionsViewModel optionsViewModel)
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
            string wandboxUrl = githubToWandbox.Transform(description);
            return Redirect(wandboxUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
