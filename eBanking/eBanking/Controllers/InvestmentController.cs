using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FusionCharts.Visualization;
using System.Data;
using FusionCharts.DataEngine;
using eBanking.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eBanking.Controllers
{
    public class InvestmentController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public InvestmentController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            var data = _dbContext.CurrencyRateHistory
                .Where(x => x.CurrencyId == 29)
                .Where(x => x.Date.Year == 2020 && x.Date.Month == 4)
                .ToList();

            // create data table to store data
            DataTable ChartData = new DataTable();

            // Add columns to data table
            ChartData.Columns.Add("Rate", typeof(System.Double));
            ChartData.Columns.Add("Date", typeof(System.String));

            // Add rows to data table
            foreach (var history in data)
            {
                ChartData.Rows.Add(history.Rate, history.Date.ToString("MM.dd"));
            }

            // Create static source with this data table
            StaticSource source = new StaticSource(ChartData);
            // Create instance of DataModel class
            DataModel model = new DataModel();
            // Add DataSource to the DataModel
            model.DataSources.Add(source);
            // Instantiate Column Chart
            Charts.ColumnChart column = new Charts.ColumnChart("first_chart");

            column.Values.PlotHighlightEffect = "color = #ffff00";
            // Set Chart's width and height
            column.Width.Pixel(700);
            column.Height.Pixel(1000);
            // Set DataModel instance as the data source of the chart
            column.Data.Source = model;
            // Set Chart Title
            column.Caption.Text = "Investment consulting";
            // hide chart Legend
            column.Legend.Show = false;
            // set XAxis Text
            column.XAxis.Text = "Dates";
            // Set YAxis title
            column.YAxis.Text = "Rates";
            // set chart theme
            column.ThemeName = FusionChartsTheme.ThemeName.CANDY;
            // Configure export settings
            column.Export.Enabled = true;
            column.Export.ExportedFileName = "fusioncharts.net_visualizations_exported_files";
            column.Export.Action = Exporter.ExportAction.DOWNLOAD;
            // set chart rendering json
            var chart = new Models.InvestmentChart
            {
                ChartJson = column.Render()
            };
            return View(chart);
        }
    }
}
