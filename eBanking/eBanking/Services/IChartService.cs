using eBanking.Models;

namespace eBanking.Services
{
    public interface IChartService
    {
        string PrepareInvestmentChart(InvestmentChart investmentChart);
    }
}
