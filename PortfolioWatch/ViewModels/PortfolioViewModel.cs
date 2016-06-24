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
            Name = portfolio.Name;
            Positions = new ObservableCollection<PositionViewModel>( portfolio.Positions.Select(_ => new PositionViewModel(_)).ToList());
        }

        #endregion

        #region Methods

        public Portfolio GetModel()
        {
            Portfolio p = new Portfolio();
            p.Name = Name;
            p.Positions = this.Positions.Select(_ => _.GetModel()).ToList();

            return p;
        }

        #endregion


    }
}
