using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioWatchBO.Containers
{
    public class DailyTicker
    {
        public string Ticker { get; set; }

        //assuming the market price is the same as AdjClose for now.
        public decimal MarketPrice { get; set; }

        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        public decimal Volume { get; set; }

        public decimal AdjClose { get; set; }

        public DateTime Date { get; set; }
    }
}
