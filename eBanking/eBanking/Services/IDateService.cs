using System;
using System.Collections.Generic;

namespace eBanking.Services
{
    public interface IDateService
    {
        IEnumerable<DateTime> EachDay(DateTime from, DateTime thru);
        DateTime StringToDateTime(string input);
        string DateTimeToString(DateTime date);
    }
}
