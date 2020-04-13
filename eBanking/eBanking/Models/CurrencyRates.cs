using System;
using System.Collections.Generic;

namespace eBanking.Models
{
    public class CurrencyRates
    {
            public Dictionary<String,Double> rates { get; set; }
            public string @base { get; set; }
            public string date { get; set; }
     }
}
