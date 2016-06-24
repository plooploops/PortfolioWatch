using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioWatchBO.Containers
{
    public class Position
    {
        public string Ticker { get; set; }

        public DateTime OpeningDate { get; set; }

        public decimal Size { get; set; }

        public decimal Price { get; set; }
    }
}
