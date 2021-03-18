using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FusionCharts.Visualization;
using System.Data;
using FusionCharts.DataEngine;
using eBanking.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using eBanking.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using eBanking.BusinessModels;

namespace eBanking.Controllers
{
    public class InvestmentController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public InvestmentController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Currency> GetCurrencyList()
        {
            return _dbContext.Currencies.ToList();
        }

        public void PopulateCurrencies()
        {
            var currencies = GetCurrencyList();
            ViewData["Currencies"] = new SelectList(currencies.OrderBy(x => x.Name), "Id", "Name");
            ViewData["Today"] = DateTime.Today.ToString("yyyy-MM-dd");
        }
        public IActionResult Index()
        {
            PopulateCurrencies();
            var investmentChart = new InvestmentChart
            {
                FirstCurrency = 3,
                SecondCurrency = 14,
                StartingDate = DateTime.Now.AddDays(-90),
                EndingDate = DateTime.Now.AddDays(60),
                RadioResponse = "LinearRegression",
                Alpha = 0.5
            };
            return View(investmentChart);
        }

        public IActionResult ShowGraph(InvestmentChart investmentChart)
        {
            DataModel model = new DataModel();
            List<ChartDataValues> _data = new List<ChartDataValues>();

            var data1 = _dbContext.CurrencyRateHistory
                    .Include(a => a.Currency)
                    .OrderBy(x => x.Date)
                    .Where(x => x.CurrencyId == investmentChart.FirstCurrency
                        && x.Date >= investmentChart.StartingDate)
                    .ToList();

            var data2 = _dbContext.CurrencyRateHistory
                .Include(a => a.Currency)
                .OrderBy(x => x.Date)
                .Where(x => x.CurrencyId == investmentChart.SecondCurrency
                    && x.Date >= investmentChart.StartingDate)
                .ToList();

            int previousValues = data2.Count();

            for (int i = 0; i < previousValues; i++)
            {
                if (i != previousValues-1) 
                {
                    _data.Add(new ChartDataValues()
                    {
                        Date = data1[i].Date.ToString("dd.MM.yyyy"),
                        FirstCurrencyValue = data1[i].Rate * 100,
                        SecondCurrencyValue = data2[i].Rate * 100
                    });
                }
                else 
                {
                    double temp = 0;
                    if(data1[i].Rate > data2[i].Rate)
                    {
                        temp = data1[i].Rate * 100;
                    }
                    else
                    {
                        temp = data2[i].Rate * 100;
                    }
                    _data.Add(new ChartDataValues()
                    {
                        Date = data1[i].Date.ToString("dd.MM.yyyy"),
                        FirstCurrencyValue = data1[i].Rate * 100,
                        SecondCurrencyValue = data2[i].Rate * 100,
                        ThirdCurrencyValue = temp
                    });
                }
                
            }

            if (investmentChart.RadioResponse.Equals("LinearPrediction"))
            {
                TimeSpan span = investmentChart.EndingDate.Subtract(DateTime.Now);
                TimeSpan decrement = DateTime.Now.Subtract(investmentChart.StartingDate);

                double FirstIncrement =
                    (data1[previousValues - 1].Rate - data1[0].Rate) / decrement.Days;
                double SecondIncrement =
                    (data2[previousValues - 1].Rate - data2[0].Rate) / decrement.Days;

                for (int i = 1; i <= span.Days; i++)
                {
                    _data.Add(new ChartDataValues()
                    {
                        Date = data1[previousValues - 1].Date.AddDays(i).ToString("dd.MM.yyyy"),
                        FirstCurrencyValue = data1[previousValues - 1].Rate * 100 + FirstIncrement * i,
                        SecondCurrencyValue = data2[previousValues - 1].Rate * 100 + SecondIncrement * i
                    });
                }
            }


            if (investmentChart.RadioResponse.Equals("LinearRegression"))
            {
                TimeSpan span = investmentChart.EndingDate.Subtract(DateTime.Now);
                TimeSpan decrement = DateTime.Now.Subtract(investmentChart.StartingDate);

                double alpha1, beta1;
                List<double> rates1 = new List<double>();
                for (int i = 0; i < previousValues; i++)
                {
                    rates1.Add(data1[i].Rate * 100);
                }

                alpha1 = LinearRegression(previousValues, rates1).ElementAt(0);
                beta1 = LinearRegression(previousValues, rates1).ElementAt(1);

                double alpha2, beta2;
                List<double> rates2 = new List<double>();
                for (int i = 0; i < previousValues; i++)
                {
                    rates2.Add(data2[i].Rate * 100);
                }

                alpha2 = LinearRegression(previousValues, rates2).ElementAt(0);
                beta2 = LinearRegression(previousValues, rates2).ElementAt(1);

                
                for (int i = 1; i <= span.Days; i++)
                {
                    
                    _data.Add(new ChartDataValues()
                    {
                        Date = data1[previousValues - 1].Date.AddDays(i).ToString("dd.MM.yyyy"),
                        FirstCurrencyValue = alpha1 + beta1 * (decrement.Days + i),
                        SecondCurrencyValue = alpha2 + beta2 * (decrement.Days + i)                        
                    });                    
                }
            }

            if (investmentChart.RadioResponse.Equals("ExponentialSmoothing"))
            {
                TimeSpan span = investmentChart.EndingDate.Subtract(DateTime.Now);

                List<double> parameters = AlphaParameters(investmentChart.Alpha);
                int countParameters = parameters.Count();
                if (countParameters > data1.Count())
                {
                    countParameters = data1.Count();
                }

                var values1 = data1.TakeLast(countParameters).Reverse().ToList();
                var values2 = data2.TakeLast(countParameters).Reverse().ToList();
                for(int i = 1; i < span.Days; i++) 
                {
                    double firstValue = 0;
                    double secondValue = 0;
                    for (int j = 1; j < countParameters; j++)
                    {
                        firstValue += parameters.ElementAt(j - 1) * values1[j - 1].Rate;
                        secondValue += parameters.ElementAt(j - 1)  * values2[j - 1].Rate;
                    }
                    values1.Insert(0, new BusinessModels.CurrencyRateHistory { Rate = firstValue });
                    values2.Insert(0, new BusinessModels.CurrencyRateHistory { Rate = secondValue });

                    _data.Add(new ChartDataValues()
                    {
                        Date = data1[previousValues - 1].Date.AddDays(i).ToString("dd.MM.yyyy"),
                        FirstCurrencyValue = firstValue * 100 ,
                        SecondCurrencyValue = secondValue * 100
                    });
                }
            }

            string json = JsonConvert.SerializeObject(_data.ToArray());
            string filePath = @"D:\eBanking\eBanking\eBanking\investmentChartData.json";
            if (!System.IO.File.Exists(filePath))
            {
                var fs = System.IO.File.Create(filePath);
                fs.Close();
            }
            System.IO.File.WriteAllText(filePath, json);
            JsonFileSource jsonFileSource = new JsonFileSource(filePath);
            model.DataSources.Add(jsonFileSource);

            Charts.CombinationChart combiChart = new Charts.CombinationChart("mscombi2d");
            combiChart.Data.Source = model;
            combiChart.Data.LinePlots("FirstCurrencyValue");
            combiChart.Data.LinePlots("SecondCurrencyValue");
            combiChart.Data.ColumnPlots("ThirdCurrencyValue");
            combiChart.XAxis.Text = "Date";
            combiChart.Caption.Text = "Investment consulting";
            combiChart.Width.Pixel(800);
            combiChart.Height.Pixel(500);

            combiChart.Data.SecondaryYAxisAsParent("SecondCurrencyValue");
            combiChart.PrimaryYAxis.Text = data1[0].Currency.Name.ToString();
            combiChart.SecondaryYAxis.Text = data2[0].Currency.Name.ToString();
            combiChart.DualY = true;

            combiChart.ThemeName = FusionChartsTheme.ThemeName.FUSION;

            var chart = new Models.ChartDataString
            {
                ChartJson = combiChart.Render()
            };
            ViewData["ChartJson"] = chart.ChartJson;
            PopulateCurrencies();
            return View("Index", investmentChart);            
        }

        private List<double> AlphaParameters(double alpha)
        {
            if (alpha < 0.1)
                alpha = 0.1;
            if (alpha > 0.9)
                alpha = 0.9;
                
            List<double> res = new List<double>();
            double multiple = 1;
            while (multiple > 0.00001)
            {
                res.Add(alpha*multiple);
                multiple *= 1 - alpha;
            }
            return res;
        }

        private List<double> LinearRegression(int NumDays, List<double> RateValues)
        {
            int xValues = 0;
            for (int i = 1; i <= NumDays; i++)
            {
                xValues += i;
            }
            double xMean = xValues * 1.0 / NumDays;

            double yValues = 0.0;
            for (int i = 0; i < NumDays; i++)
            {
                yValues += RateValues.ElementAt(i);
            }
            double yMean = yValues * 1.0/ NumDays;

            double a = 0;
            for (int i = 0; i < NumDays; i++)
            {
                a += (i + 1 - xMean) * (RateValues.ElementAt(i) - yMean);
            }

            double b = 0;
            for (int i = 0; i < NumDays; i++)
            {
                b += (i + 1 - xMean) * (i + 1 - xMean);
            }

            double beta = a *1.0 / b;
            double alpha = yMean - beta * xMean;

            List<double> res = new List<double>();
            res.Add(alpha);
            res.Add(beta);

            res.ToArray();

            return res;
        }
    }
}