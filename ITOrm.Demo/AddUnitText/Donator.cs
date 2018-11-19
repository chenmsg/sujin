using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddUnitText
{
    public class Donator
    {
        public int DonatorId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime DonateDate { get; set; }
    }

    public class PayWay
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
