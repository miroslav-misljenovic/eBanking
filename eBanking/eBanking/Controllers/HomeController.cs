using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using eBanking.Models;
using System.Net.Http;
using eBanking.BusinessModels;
using eBanking.Data;
using Microsoft.AspNetCore.Identity;

namespace eBanking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            CurrencyRates cr = RefreshRates();
            return View(cr);
        }

        public CurrencyRates RefreshRates()
        {
            CurrencyRates cr = new CurrencyRates { rates = new Dictionary<string, double>() };
            DateTime start = new DateTime(2020, 01, 01);
            var lastDate = _dbContext.CurrencyRateHistory.OrderBy(x => x.Date).LastOrDefault();
            if (lastDate != null)
            {
                start = lastDate.Date;
            }

            using (var httpClient = new HttpClient())
            {
                foreach (DateTime day in EachDay(start, DateTime.Today))
                {
                    try
                    {
                        var dayString = day.Year + "-" + day.Month + "-" + day.Day;
                        var response = httpClient.GetAsync("https://api.exchangeratesapi.io/" + dayString).Result;
                        var a = response.Content.ReadAsStringAsync().Result;
                        var rates = Newtonsoft.Json.JsonConvert.DeserializeObject<CurrencyRates>(a);
                        cr = rates;
                        foreach (var c in rates.rates)
                        {
                            var currency = _dbContext.Currencies.SingleOrDefault(x => x.Name == c.Key);
                            if (currency == null)
                            {
                                currency = new Currency { Name = c.Key };
                                _dbContext.Currencies.Add(currency);
                                _dbContext.SaveChanges();
                            }
                            if (rates.date == DateTime.Today.ToString("yyyy-MM-dd"))
                            {
                                currency.Rate = c.Value;
                            }
                            DateTime date = StringToDateTime(rates.date);
                            if (!_dbContext.CurrencyRateHistory.AsEnumerable()
                                .Any(x => x.CurrencyId == currency.Id && x.Date.ToString("yyyy-MM-dd") == rates.date))
                            {
                                _dbContext.CurrencyRateHistory.Add(new CurrencyRateHistory
                                {
                                    CurrencyId = currency.Id,
                                    Date = date,
                                    Rate = c.Value,
                                });
                            }
                        }
                        _dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    { 
                        
                    }
                }
            }
            return cr;
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        private DateTime StringToDateTime(string input)
        {
            string[] temp = input.Split('-');
            return new DateTime(int.Parse(temp[0]), int.Parse(temp[1]), int.Parse(temp[2]));
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
