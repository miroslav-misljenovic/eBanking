using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using eBanking.Models;
using System.Net.Http;

namespace eBanking.Controllers
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
            CurrencyRates cr = new CurrencyRates { rates = new Dictionary<string, double>()};
            try { 
            using(var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync("https://api.exchangeratesapi.io/latest").Result;
                var a = response.Content.ReadAsStringAsync().Result;
                cr = Newtonsoft.Json.JsonConvert.DeserializeObject<CurrencyRates>(a);
            }
            }
            catch { }
            return View(cr);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
