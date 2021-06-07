using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using eBanking.Models;
using System;
using eBanking.Services;

namespace eBanking.Controllers
{
    public class InvestmentController : Controller
    {
        private readonly IChartService _chartService;
        private readonly ICurrencyRateService _currencyRateService;

        public InvestmentController(IChartService chartService,
            ICurrencyRateService currencyRateService)
        {
            _chartService = chartService ?? throw new ArgumentException(nameof(chartService));
            _currencyRateService = currencyRateService ?? throw new ArgumentException(nameof(currencyRateService));
        }
        public void PopulateCurrencies()
        {
            var currencies = _currencyRateService.GetCurrencyList();
            ViewData["Currencies"] = new SelectList(currencies.OrderBy(x => x.Name), "Id", "Name");
            ViewData["Today"] = DateTime.Today.ToString("yyyy-MM-dd");
        }
        public IActionResult Index()
        {
            PopulateCurrencies();

            var currencies = _currencyRateService.GetCurrencyList();
            int currency1 = currencies.First(a => a.Name == "VEF").Id;
            int currency2 = currencies.First(a => a.Name == "RSD").Id;

            var investmentChart = new InvestmentChart
            {
                FirstCurrency = currency1,
                SecondCurrency = currency2,
                StartingDate = DateTime.Now.AddDays(-70),
                EndingDate = DateTime.Now.AddDays(30),
                RadioResponse = "LinearRegression",
                Alpha = 0.5
            };            
            ViewData["ChartJson"] = _chartService.PrepareInvestmentChart(investmentChart);
            return View(investmentChart);
        }
        public IActionResult ShowGraph(InvestmentChart investmentChart)
        {
            ViewData["ChartJson"] = _chartService.PrepareInvestmentChart(investmentChart);
            PopulateCurrencies();
            return View("Index", investmentChart);            
        }        
    }
}