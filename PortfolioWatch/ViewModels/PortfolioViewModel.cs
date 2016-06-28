using PortfolioWatchBO.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PortfolioWatch.Commands;
using System.Collections.ObjectModel;

namespace PortfolioWatch.ViewModels
{
    public class PortfolioViewModel : BaseViewModel
    {
        #region Properties

        public string Name { get; set; }

        public ObservableCollection<PositionViewModel> Positions { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PortfolioViewModel()
        {

        }

        public PortfolioViewModel(Portfolio portfolio)
        {
            try
            {
                if (portfolio != null)
                {
                    Name = portfolio.Name;
                    if (portfolio.Positions != null)
                    {
                        Positions = new ObservableCollection<PositionViewModel>(portfolio.Positions.Select(_ => new PositionViewModel(_)).ToList());
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region Methods

        public Portfolio GetModel()
        {
            try
            {
                Portfolio p = new Portfolio();
                p.Name = Name;
                p.Positions = this.Positions.Select(_ => _.GetModel()).ToList();

                return p;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Portfolio() { Name = Name, Positions = new List<Position>() };
            }
        }

        #endregion


    }
}
