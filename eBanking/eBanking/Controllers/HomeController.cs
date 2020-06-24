using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using eBanking.Models;
using System.Net.Http;
using eBanking.BusinessModels;
using eBanking.Data;
using Microsoft.AspNetCore.Identity;

namespace eBanking.Controllers
{
    public class HomeController : Controller
    {
        private readonly int EUR_ID = 1;
        private readonly ApplicationDbContext _dbContext;
               
        public HomeController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            RefreshRates();

            CurrencyRates cr = new CurrencyRates { rates = new Dictionary<string, double>() };

            using (var httpClient = new HttpClient()) 
            {
                try {
                    var response = httpClient.GetAsync("https://api.exchangeratesapi.io/latest").Result;
                    var a = response.Content.ReadAsStringAsync().Result;
                    cr = Newtonsoft.Json.JsonConvert.DeserializeObject<CurrencyRates>(a);
                    cr.date = DateTimeToString(DateTime.Today);
                }
                catch (Exception ex) { }
                
            }
            return View(cr);
        }

        public void RefreshRates()
        {
            DateTime start = new DateTime(2020, 1, 1);
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
                        var dayString = DateTimeToString(day);
                        var response = httpClient.GetAsync("https://api.exchangeratesapi.io/" + dayString).Result;
                        var a = response.Content.ReadAsStringAsync().Result;
                        var rates = Newtonsoft.Json.JsonConvert.DeserializeObject<CurrencyRates>(a);
                        foreach (var c in rates.rates)
                        {
                            var currency = _dbContext.Currencies.SingleOrDefault(x => x.Name == c.Key);
                            if (currency == null)
                            {
                                currency = new Currency { Name = c.Key };
                                _dbContext.Currencies.Add(currency);
                                _dbContext.SaveChanges();
                            }
                            if (dayString == DateTime.Today.ToString("yyyy-MM-dd"))
                            {
                                currency.Rate = c.Value;
                            }

                            if (!_dbContext.CurrencyRateHistory.AsEnumerable()
                                .Any(x => (x.CurrencyId == currency.Id) && (x.Date.ToString("yyyy-MM-dd") == dayString)))
                            {
                                _dbContext.CurrencyRateHistory.Add(new CurrencyRateHistory
                                {
                                    CurrencyId = currency.Id,
                                    Date = StringToDateTime(dayString),
                                    Rate = c.Value,
                                });
                            }

                        }

                        if (!_dbContext.CurrencyRateHistory.AsEnumerable()
                                .Any(x => (x.CurrencyId == EUR_ID) && (x.Date.ToString("yyyy-MM-dd") == dayString)))
                        {
                            _dbContext.CurrencyRateHistory.Add(new CurrencyRateHistory
                            {
                                CurrencyId = EUR_ID,
                                Date = StringToDateTime(dayString),
                                Rate = 1,
                            });
                        }
                        _dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
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

        private string DateTimeToString(DateTime date)
        {
            string month = date.Month > 9 ? $"{date.Month}" : $"0{date.Month}";
            string day = date.Day > 9 ? $"{date.Day}" : $"0{date.Day}";
            return $"{date.Year}-{month}-{day}";
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
