using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioWatchBO.Containers
{
    public class MarketHistory
    {
        public List<string> TickerNames { get; set; }

        public Dictionary<string, List<DailyTicker>> TickerMap { get; set; }
    }
}
