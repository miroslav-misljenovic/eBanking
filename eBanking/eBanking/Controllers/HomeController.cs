using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBanking.Models;
using eBanking.Services;

namespace eBanking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICurrencyRateService _currencyRateService;
        public HomeController(
            ICurrencyRateService currencyRateService)
        {
            _currencyRateService = currencyRateService ?? throw new ArgumentException(nameof(currencyRateService));
        }
        public IActionResult Index()
        {
            CurrencyRates cr = _currencyRateService.GetCurrencyRate();
            return View(cr);
        }

        [HttpGet, Route("RefreshRates")]
        public IActionResult RefreshRates()
        {
            _currencyRateService.RefreshRates();
            return Index();
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
