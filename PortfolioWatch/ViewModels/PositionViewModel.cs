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
                if (value >= 0)
                {
                    Model.Size = value;
                }
                else
                {
                    //force validation against negative shares.
                    Model.Size = 0;
                }
                NotifyPropertyChanged("Size");
                NotifyPropertyChanged("CurrentHoldingCost");
                NotifyPropertyChanged("CurrentHoldingValue");
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
                NotifyPropertyChanged("DailyDifference");
                NotifyPropertyChanged("InceptionToDateDifference");
                NotifyPropertyChanged("CurrentHoldingCost");
                NotifyPropertyChanged("CurrentHoldingValue");
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
                if (InceptionModel != null && TodayModel != null)
                    return TodayModel.Price - InceptionModel.Price;
                else
                    return 0;
            }
        }

        public decimal CurrentHoldingCost
        {
            get
            {
                return Model.Price * Model.Size;
            }
        }

        public decimal CurrentHoldingValue
        {
            get
            {
                if (TodayModel != null)
                    return TodayModel.Price * Model.Size;
                else
                    return 0;
            }
        }

        Position Model { get; set; }

        private Position _todayModel;
        public Position TodayModel
        {
            get
            {
                return _todayModel;
            }
            set
            {
                _todayModel = value;
                NotifyPropertyChanged("CurrentHoldingValue");
            }
        }


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
