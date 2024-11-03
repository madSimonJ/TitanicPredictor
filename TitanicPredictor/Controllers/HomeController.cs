using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TitanicPredictor.Models;

namespace TitanicPredictor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [Route("Home/Result/{yesOrNo:bool}")]
        public IActionResult Result(bool yesOrNo)
        {
            return View(new PredictionResultModel
            {
                YesOrNo = yesOrNo
            });
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
