using eBanking.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace eBanking.Controllers
{
    public class ExchangeRateController : Controller
    {
        private readonly IExchangeRatesService _exchangeRatesService;
        public ExchangeRateController(IExchangeRatesService exchangeRatesService)
        {
            _exchangeRatesService = exchangeRatesService ?? throw new ArgumentException(nameof(exchangeRatesService));
        }
        public IActionResult Index()
        {
            return View();
        }        
        public IActionResult GenerateList(Models.ExchangeRateRequest req)
        {
            ViewData["Rates"] = _exchangeRatesService.PrepareRates(req);
            return View("Index");
        }
    }
}
