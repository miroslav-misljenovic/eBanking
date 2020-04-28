using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eBanking.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
    }
}
