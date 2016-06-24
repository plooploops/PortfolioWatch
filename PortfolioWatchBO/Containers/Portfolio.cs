using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioWatchBO.Containers
{
    public class Portfolio
    {
        public string Name { get; set; }

        public List<Position> Positions { get; set; }
    }
}
