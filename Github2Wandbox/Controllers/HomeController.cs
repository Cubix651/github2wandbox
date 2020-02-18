using System.Diagnostics;
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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly PublicationUrlGenerator publishUrlGenerator;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;

            publishUrlGenerator = new PublicationUrlGenerator();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
