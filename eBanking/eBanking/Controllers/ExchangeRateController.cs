using eBanking.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eBanking.Controllers
{
    public class ExchangeRateController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public ExchangeRateController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult GenerateList(Models.ExchangeRateRequest req)
        {
            var a = _dbContext.CurrencyRateHistory
                .Include(a => a.Currency)
                .Where(a => a.Date.Year == req.Date.Year && 
                            a.Date.Month == req.Date.Month &&
                            a.Date.Day == req.Date.Day)
                .ToList();
            var result = a.Select(x => new Models.ExchangeRateOnDate { Name = x.Currency.Name, Rate = x.Rate }).ToList();
            ViewData["Rates"] = result;  

            return View("Index");
        }

        private DateTime StringToDateTime(string input)
        {
            string[] temp = input.Split('-');
            return new DateTime(int.Parse(temp[0]), int.Parse(temp[1]), int.Parse(temp[2]));
        }
    }
}
