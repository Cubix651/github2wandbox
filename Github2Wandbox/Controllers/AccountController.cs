using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Github2Wandbox.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Github2Wandbox.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            this.logger = logger;
            this.userManager = userManager;
            this.signInManager = signInManager;
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
                    return RedirectToAction("Index", "Home");
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
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("", "Invalid login attempt");
            }
            return View(viewModel);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
