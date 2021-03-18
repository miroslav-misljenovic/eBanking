﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using eBanking.Models;
using System.Net.Http;
using eBanking.BusinessModels;
using eBanking.Data;

namespace eBanking.Services
{
    public class CurrencyRateService : ICurrencyRateService
    {
        private readonly int EUR_ID = 1;

        private readonly ApplicationDbContext _dbContext;
        private readonly IDateService _dateService;

        public CurrencyRateService(ApplicationDbContext dbContext,
            IDateService dateService)
        {
            _dbContext = dbContext;
            _dateService = dateService;
        }

        public CurrencyRates GetCurrencyRate()
        {
            RefreshRates();
            CurrencyRates cr = new CurrencyRates { rates = new Dictionary<string, double>() };

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = httpClient.GetAsync("https://api.exchangeratesapi.io/latest").Result;
                    var a = response.Content.ReadAsStringAsync().Result;
                    cr = Newtonsoft.Json.JsonConvert.DeserializeObject<CurrencyRates>(a);
                    cr.date = _dateService.DateTimeToString(DateTime.Today);
                }
                catch (Exception ex)
                {
                    // logovati sve ovakve stvari
                }

            }

            return cr;
        }

        public void RefreshRates()
        {
            DateTime start = new DateTime(2021, 3, 1);
            var lastDate = _dbContext.CurrencyRateHistory.OrderBy(x => x.Date).LastOrDefault();
            if (lastDate != null)
            {
                start = lastDate.Date;
            }

            using (var httpClient = new HttpClient())
            {
                foreach (DateTime day in _dateService.EachDay(start, DateTime.Today))
                {
                    try
                    {
                        var dayString = _dateService.DateTimeToString(day);
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
                                    Date = _dateService.StringToDateTime(dayString),
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
                                Date = _dateService.StringToDateTime(dayString),
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

    }
}
