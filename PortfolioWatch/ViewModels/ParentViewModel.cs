using PortfolioWatch.Commands;
using PortfolioWatchBO.Containers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace PortfolioWatch.ViewModels
{
    public class ParentViewModel : BaseViewModel
    {
        #region Properties

        private DelegateCommand exitCommand;
        private Action<string> popup;
        private Func<string, string, bool> confirm;

        private MarketHistoryViewModel _marketHistoryViewModel;
        public MarketHistoryViewModel MarketHistoryViewModel
        {
            get
            {
                return _marketHistoryViewModel;
            }
            set
            {
                _marketHistoryViewModel = value;
                NotifyPropertyChanged("MarketHistoryViewModel");
            }
        }

        private PortfolioViewModel _portfolioViewModel;
        public PortfolioViewModel PortfolioViewModel
        {
            get
            {
                return _portfolioViewModel;
            }
            set
            {
                _portfolioViewModel = value;
                NotifyPropertyChanged("PortfolioViewModel");
            }
        }

        private string _filePath;
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                NotifyPropertyChanged("FilePath");
            }
        }

        #endregion

        #region Constructor

        public ParentViewModel()
        {
            MarketHistoryViewModel = new MarketHistoryViewModel();
            PortfolioViewModel = new PortfolioViewModel();

            InitializeViewModels();
        }

        public ParentViewModel(Action<string> popup, Func<string, string, bool> confirm)
        {
            MarketHistoryViewModel = new MarketHistoryViewModel();
            PortfolioViewModel = new PortfolioViewModel();

            InitializeViewModels();

            this.popup = popup;
            this.confirm = confirm;
        }

        #endregion

        #region Methods

        public void TryShowPopup(string message)
        {
            if (popup != null && !string.IsNullOrEmpty(message))
                popup.Invoke(message);
        }

        public void InitializeViewModels()
        {
            try
            {
                string errMsg = string.Empty;
                Task parentTask = Task.Run(() =>
                {
                    try
                    {

                        MarketHistoryViewModel = new MarketHistoryViewModel(new List<string>()
                        {
                            "MSFT", "AAPL", "GOOG", "CVX"
                        }, DateTime.Now.AddDays(-20));
                        MarketHistoryViewModel.RefreshMarketHistory();
                        if (MarketHistoryViewModel.History == null || MarketHistoryViewModel.History.Count == 0)
                        {
                            TryShowPopup("Couldn't load history.");
                            return;
                        }
                        PortfolioViewModel = new PortfolioViewModel(new Portfolio()
                        {
                            Name = "Test Portfolio",
                            Positions = new List<Position>() {
                                new Position(){
                                   OpeningDate = DateTime.Today,
                                   Price = MarketHistoryViewModel.History.Where(_ => _.Ticker == "MSFT").OrderByDescending(h => h.Date).First().AdjClose,
                                   Size = 123,
                                   Ticker = "MSFT"
                                },
                                new Position(){
                                   OpeningDate = DateTime.Today,
                                   Price = MarketHistoryViewModel.History.Where(_ => _.Ticker == "AAPL").OrderByDescending(h => h.Date).First().AdjClose,
                                   Size = 321,
                                   Ticker = "AAPL"
                                },
                                new Position(){
                                   OpeningDate = DateTime.Today,
                                   Price = MarketHistoryViewModel.History.Where(_ => _.Ticker == "CVX").OrderByDescending(h => h.Date).First().AdjClose,
                                   Size = 1232,
                                   Ticker = "CVX"
                                },
                                new Position(){
                                   OpeningDate = DateTime.Today,
                                   Price = MarketHistoryViewModel.History.Where(_ => _.Ticker == "GOOG").OrderByDescending(h => h.Date).First().AdjClose,
                                   Size = 1242,
                                   Ticker = "GOOG"
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        errMsg = "Issue with initializing portfolio!";
                        Console.WriteLine(ex.ToString());
                    }
                });

                parentTask.Wait();
                UpdatePortfolioCalculations();
            }
            catch (Exception ex)
            {
                TryShowPopup("Couldn't load initial portfolio!");
                Console.WriteLine(ex.ToString());
            }
        }

        public void RefreshViewModels()
        {
            try
            {
                MarketHistoryViewModel.RefreshMarketHistory();
                PortfolioViewModel = new PortfolioViewModel();
            }
            catch (Exception ex)
            {
                TryShowPopup("Couldn't refresh market data!");
                Console.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region Commands

        public ICommand ExitCommand
        {
            get
            {
                if (exitCommand == null)
                    exitCommand = new DelegateCommand(Exit);
                return exitCommand;
            }
        }

        private void Exit()
        {
            if (confirm("Are you sure you want to exit", "confirm exit"))
                Application.Current.Shutdown();
        }

        public ICommand RefreshMarketHistoryClickCommand
        {
            get { return new DelegateCommand(RefreshMarketHistory); }
        }

        private void UpdatePortfolioCalculations()
        {
            try
            {
                string errMsg = String.Empty;
                Task parentTask = Task.Run(() =>
                {
                    try
                    {
                        foreach (var pos in PortfolioViewModel.Positions)
                        {
                            var relevantHistory = MarketHistoryViewModel.History.Where(_ => _.Ticker == pos.Ticker && _.Date < pos.OpeningDate).OrderByDescending(_ => _.Date).ToList();
                            if (relevantHistory.Count == 0)
                                continue;
                            var yesterdayHistory = relevantHistory.First();

                            var yesterdayPosition = new Position() { OpeningDate = yesterdayHistory.Date, Price = yesterdayHistory.MarketPrice, Ticker = pos.Ticker };

                            pos.YesterdayModel = yesterdayPosition;

                            var currentHistory = MarketHistoryViewModel.History.Where(_ => _.Ticker == pos.Ticker && _.Date >= pos.OpeningDate).OrderBy(_ => _.Date).ToList();
                            pos.Price = currentHistory.First().AdjClose;
                            var today = MarketHistoryViewModel.History.Where(_ => _.Ticker == pos.Ticker).OrderByDescending(_ => _.Date).ToList();
                            pos.TodayModel = new Position() { OpeningDate = today.First().Date, Price = today.First().MarketPrice, Ticker = pos.Ticker };

                            var inceptionHistory = today.Last();
                            var inceptionPosition = new Position() { OpeningDate = inceptionHistory.Date, Price = inceptionHistory.MarketPrice, Ticker = pos.Ticker };
                            pos.InceptionModel = inceptionPosition;
                        }
                    }
                    catch (Exception ex)
                    {
                        errMsg = "Could not calculate portfolio!";
                        Console.WriteLine(ex.ToString());
                    }
                });
                parentTask.Wait();
                TryShowPopup(errMsg);
            }
            catch (Exception ex)
            {
                TryShowPopup("Couldn't calculate portfolio tickers!");
                Console.WriteLine(ex.ToString());
            }
        }

        private void RefreshMarketHistory()
        {
            try
            {
                string errMsg = String.Empty;
                Task parentTask = Task.Run(() =>
                {
                    try
                    {
                        MarketHistoryViewModel.RefreshMarketHistory(PortfolioViewModel.Positions.Select(_ => _.Ticker).Distinct().ToList());

                        UpdatePortfolioCalculations();
                    }
                    catch (Exception ex)
                    {
                        errMsg = "Could not load the market data.  Please ensure connectivity (and that yahoo finance is available!)";
                        Console.WriteLine(ex.ToString());
                    }
                });

                parentTask.Wait();
                TryShowPopup(errMsg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public ICommand AddPositionNewRowClickCommand
        {
            get { return new DelegateCommand(AddPosition); }
        }

        private void AddPosition()
        {
            if (PortfolioViewModel != null && PortfolioViewModel.Positions != null)
                PortfolioViewModel.Positions.Add(new PositionViewModel());
            //this is called when the button is clicked
        }

        private void LoadFromFile()
        {
            //load data from .xml file
            try
            {
                XmlSerializer x = new XmlSerializer(typeof(Portfolio));
                if (!File.Exists(FilePath) && Path.GetExtension(FilePath.Trim().ToLower()) == ".xml")
                {
                    TryShowPopup("Please ensure that the file exists and that the file ends with .xml");
                    //need to pick a different file.
                    return;
                }
                using (FileStream myFileStream = new FileStream(FilePath, FileMode.Open))
                {
                    // Call the Deserialize method and cast to the object type.
                    var portfolio = (Portfolio)x.Deserialize(myFileStream);
                    PortfolioViewModel = new PortfolioViewModel(portfolio);
                }

                UpdatePortfolioCalculations();

                TryShowPopup("Loaded portfolio!");
            }
            catch (Exception ex)
            {
                TryShowPopup("Issue with loading portfolio from file.  Please be sure to select the .xml file and make sure it's not open!");
                Console.WriteLine(ex.ToString());
            }
        }

        public ICommand LoadPortfolioClickCommand
        {
            get { return new DelegateCommand(LoadFromFile); }
        }

        public ICommand SavePortfolioClickCommand
        {
            get { return new DelegateCommand(SaveToFile); }
        }

        private void SaveToFile()
        {
            var model = PortfolioViewModel.GetModel();
            XmlSerializer x = new XmlSerializer(model.GetType());
            if (Path.GetExtension(FilePath.Trim().ToLower()) != ".xml")
            {
                TryShowPopup("Please ensure the file ends with .xml");
                //need to pick a different file.
                return;
            }
            try
            {
                using (FileStream ms = new FileStream(FilePath, FileMode.OpenOrCreate))
                {
                    x.Serialize(ms, model);
                }
            }
            catch (Exception ex)
            {
                TryShowPopup("Please ensure that the file does not exist and that the file ends with .xml.\r\nPlease ensure file access.");
                return;
            }

            TryShowPopup("Saved portfolio");
        }

        #endregion
    }
}
