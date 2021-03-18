using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eBanking.Services
{
    public class DateService : IDateService
    {

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public DateTime StringToDateTime(string input)
        {
            string[] temp = input.Split('-');
            return new DateTime(int.Parse(temp[0]), int.Parse(temp[1]), int.Parse(temp[2]));
        }

        public string DateTimeToString(DateTime date)
        {
            string month = date.Month > 9 ? $"{date.Month}" : $"0{date.Month}";
            string day = date.Day > 9 ? $"{date.Day}" : $"0{date.Day}";
            return $"{date.Year}-{month}-{day}";
        }
    }
}
