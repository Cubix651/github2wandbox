using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Github2Wandbox.Models;
using Github2Wandbox.ViewModels;
using System.Threading.Tasks;
using Github2Wandbox.Repository;
using Microsoft.AspNetCore.Identity;
using Github2Wandbox.Models.Github;
using Github2Wandbox.Models.Wandbox;
using Github2Wandbox.Models.Communication;

namespace Github2Wandbox.Controllers
{
    public class HomeController : Controller
    {
        public static string UserAgent { get; } = "Github2Wandbox";

        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClient httpClient;
        private readonly GithubToWandbox githubToWandbox;
        private readonly PublicationUrlGenerator publishUrlGenerator;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public HomeController(ILogger<HomeController> logger,
            PublicationsContext publicationsContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;

            httpClient = new HttpClient();
            httpClient.AddUserAgent(UserAgent);

            githubToWandbox = new GithubToWandbox(
                publicationsContext,
                new GithubDirectoryCommitChecker(httpClient),
                new GithubDirectoryScanner(httpClient),
                new WandboxPublisher(httpClient));

            publishUrlGenerator = new PublicationUrlGenerator();

            this.userManager = userManager;
            this.signInManager = signInManager;
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
        public async Task<IActionResult> Publish(OptionsViewModel optionsViewModel)
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

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = viewModel.Email,
                    Email = viewModel.Email
                };
                var result = await userManager.CreateAsync(user, viewModel.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.Remember, false);
                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));

                ModelState.AddModelError("", "Invalid login attempt");
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
