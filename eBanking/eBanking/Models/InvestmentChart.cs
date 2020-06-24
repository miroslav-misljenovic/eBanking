using System;

namespace eBanking.Models
{
	public class InvestmentChart
    {
        public int FirstCurrency { get; set; }
        public int SecondCurrency { get; set; }
		public DateTime StartingDate { get; set; }
		public DateTime EndingDate { get; set; }
		public string RadioResponse { get; set; }
		public double Alpha { get; set; }
	}
}
