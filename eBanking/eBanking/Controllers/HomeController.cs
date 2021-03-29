using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBanking.Models;
using eBanking.Services;

namespace eBanking.Controllers
{
    public class HomeController : Controller
    {
        // ovaj se brise kad se svi prebace na servise
        private readonly IDateService _dateService;
        private readonly ICurrencyRateService _currencyRateService;

        public HomeController(IDateService dateService,
            ICurrencyRateService currencyRateService)
        {
            _dateService = dateService ?? throw new ArgumentException(nameof(dateService));
            _currencyRateService = currencyRateService ?? throw new ArgumentException(nameof(currencyRateService));
        }

        public IActionResult Index()
        {
            CurrencyRates cr = _currencyRateService.GetCurrencyRate();
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
