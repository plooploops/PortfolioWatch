﻿using PortfolioWatch.Commands;
using PortfolioWatchBL.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PortfolioWatch.ViewModels
{
    public class MarketHistoryViewModel : BaseViewModel
    {
        #region Properties

        public ObservableCollection<string> TickerList { get; set; }

        public DateTime FromDate { get; set; }

        private ObservableCollection<DailyTickerViewModel> _history;
        public ObservableCollection<DailyTickerViewModel> History
        {
            get
            {
                return _history;
            }
            set
            {
                _history = value;
                NotifyPropertyChanged("History");
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MarketHistoryViewModel()
        {

        }

        public MarketHistoryViewModel(List<string> tickerList, DateTime dateTime)
        {
            try
            {
                TickerList = new ObservableCollection<string>(tickerList);
                FromDate = dateTime;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region Methods

        public void RefreshMarketHistory(List<string> tickerList)
        {
            try
            {
                History = new ObservableCollection<DailyTickerViewModel>();
                TickerHelper th = new TickerHelper();

                foreach (var ticker in tickerList)
                {
                    var results = th.ReadTickerData(ticker);
                    foreach (var res in (results.Select(_ => new DailyTickerViewModel(_)).ToList()))
                    {
                        History.Add(res);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void RefreshMarketHistory()
        {
            try
            {
                History = new ObservableCollection<DailyTickerViewModel>();
                TickerHelper th = new TickerHelper();

                foreach (var ticker in TickerList)
                {
                    var results = th.ReadTickerData(ticker);
                    if (results == null)
                        continue;
                    foreach (var res in (results.Select(_ => new DailyTickerViewModel(_)).ToList()))
                    {
                        History.Add(res);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void RefreshMarketHistoryFromDate(DateTime fromDate)
        {
            try
            {
                History = new ObservableCollection<DailyTickerViewModel>();
                TickerHelper th = new TickerHelper();

                foreach (var ticker in TickerList)
                {
                    var results = th.ReadTickerData(ticker, fromDate);
                    foreach (var res in (results.Select(_ => new DailyTickerViewModel(_)).ToList()))
                    {
                        History.Add(res);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        #endregion
    }
}
