using PortfolioWatchBO.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioWatch.ViewModels
{
    public class PositionViewModel : BaseViewModel
    {
        #region Properties

        public string Ticker
        {
            get
            {
                return Model.Ticker;
            }
            set
            {
                Model.Ticker = value;
                NotifyPropertyChanged("Ticker");
            }
        }

        public DateTime OpeningDate
        {
            get
            {
                return Model.OpeningDate;
            }
            set
            {
                Model.OpeningDate = value;
                NotifyPropertyChanged("OpeningDate");
            }
        }

        public decimal Size
        {
            get
            {
                return Model.Size;
            }
            set
            {
                Model.Size = value;
                NotifyPropertyChanged("Size");
                NotifyPropertyChanged("CurrentHolding");
            }
        }

        public decimal Price
        {
            get
            {
                return Model.Price;
            }
            set
            {
                Model.Price = value;
                NotifyPropertyChanged("Price");
                NotifyPropertyChanged("CurrentHolding");
            }
        }

        public decimal DailyDifference
        {
            get
            {
                if (YesterdayModel != null)
                    return Model.Price - YesterdayModel.Price;
                else
                    return 0;
            }
        }

        public decimal InceptionToDateDifference
        {
            get
            {
                if (InceptionModel != null)
                    return Model.Price - InceptionModel.Price;
                else
                    return 0;
            }
        }

        public decimal CurrentHolding
        {
            get
            {
                return Model.Price * Model.Size;
            }
        }

        Position Model { get; set; }

        private Position _yesterdayModel;
        public Position YesterdayModel
        {
            get
            {
                return _yesterdayModel;
            }
            set
            {
                _yesterdayModel = value;
                NotifyPropertyChanged("DailyDifference");
            }
        }

        private Position _inceptionModel;
        public Position InceptionModel
        {
            get
            {
                return _inceptionModel;
            }
            set
            {
                _inceptionModel = value;
                NotifyPropertyChanged("InceptionToDateDifference");
            }
        }

        #endregion

        #region Constructor

        public PositionViewModel()
        {
            Model = new Position();
        }

        public PositionViewModel(Position pos)
        {
            Model = pos;
        }

        #endregion

        #region Methods

        public Position GetModel()
        {
            return Model;
        }

        #endregion
    }
}
