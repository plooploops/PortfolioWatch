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

        public void InitializeViewModels()
        {
            MarketHistoryViewModel = new MarketHistoryViewModel(new List<string>()
            {
                "MSFT", "AAPL", "GOOG", "CVX"
            }, DateTime.Now.AddDays(-20));
            MarketHistoryViewModel.RefreshMarketHistory();
            PortfolioViewModel = new PortfolioViewModel(new Portfolio()
            {
                Name = "Test Portfolio",
                Positions = new List<Position>() {
                    new Position(){
                       OpeningDate = DateTime.Today.AddDays(-2),
                       Price = MarketHistoryViewModel.History.Where(_=> _.Date == DateTime.Today.AddDays(-2) && _.Ticker == "MSFT").First().AdjClose,
                       Size = 123,
                       Ticker = "MSFT"
                    },
                    new Position(){
                       OpeningDate = DateTime.Today.AddDays(-2),
                       Price = MarketHistoryViewModel.History.Where(_=> _.Date == DateTime.Today.AddDays(-2) && _.Ticker == "AAPL").First().AdjClose,
                       Size = 321,
                       Ticker = "AAPL"
                    },
                    new Position(){
                       OpeningDate = DateTime.Today.AddDays(-2),
                       Price = MarketHistoryViewModel.History.Where(_=> _.Date == DateTime.Today.AddDays(-2) && _.Ticker == "CVX").First().AdjClose,
                       Size = 1232,
                       Ticker = "CVX"
                    },
                    new Position(){
                       OpeningDate = DateTime.Today.AddDays(-2),
                       Price = MarketHistoryViewModel.History.Where(_=> _.Date == DateTime.Today.AddDays(-2) && _.Ticker == "GOOG").First().AdjClose,
                       Size = 1242,
                       Ticker = "GOOG"
                    }
                }
            });
            UpdatePortfolioCalculations();
        }

        public void RefreshViewModels()
        {
            MarketHistoryViewModel.RefreshMarketHistory();
            PortfolioViewModel = new PortfolioViewModel();
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
            //update the positions based on market.
            for (int i = 0; i < 2; i++)
            {
                //update twice to account for newly added ones.
                foreach (var pos in PortfolioViewModel.Positions)
                {
                    var relevantHistory = MarketHistoryViewModel.History.Where(_ => _.Ticker == pos.Ticker && _.Date < pos.OpeningDate).OrderByDescending(_ => _.Date).ToList();
                    var yesterdayHistory = relevantHistory.First();
                    var inceptionHistory = relevantHistory.Last();

                    var yesterdayPosition = new Position() { OpeningDate = yesterdayHistory.Date, Price = yesterdayHistory.MarketPrice, Ticker = pos.Ticker };
                    var inceptionPosition = new Position() { OpeningDate = inceptionHistory.Date, Price = inceptionHistory.MarketPrice, Ticker = pos.Ticker };

                    pos.YesterdayModel = yesterdayPosition;
                    pos.InceptionModel = inceptionPosition;

                    var currentHistory = MarketHistoryViewModel.History.Where(_ => _.Ticker == pos.Ticker && _.Date >= pos.OpeningDate).OrderBy(_ => _.Date).ToList();
                    pos.Price = currentHistory.First().AdjClose;
                }
            }
        }

        private void RefreshMarketHistory()
        {
            MarketHistoryViewModel.RefreshMarketHistory(PortfolioViewModel.Positions.Select(_ => _.Ticker).Distinct().ToList());

            UpdatePortfolioCalculations();
        }

        public ICommand AddPositionNewRowClickCommand
        {
            get { return new DelegateCommand(AddPosition); }
        }

        private void AddPosition()
        {
            PortfolioViewModel.Positions.Add(new PositionViewModel());
            //this is called when the button is clicked
        }

        private void LoadFromFile()
        {
            //load data from .xml file
            XmlSerializer x = new XmlSerializer(typeof(Portfolio));
            if (!File.Exists(FilePath) && Path.GetExtension(FilePath.Trim().ToLower()) == ".xml")
            {
                popup.Invoke("Please ensure that the file exists and that the file ends with .xml");
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

            popup.Invoke("Loaded portfolio!");
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
                popup.Invoke("Please ensure the file ends with .xml");
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
                popup.Invoke("Please ensure that the file does not exist and that the file ends with .xml.\r\nPlease ensure file access.");
                return;
            }

            popup.Invoke("Saved");
        }

        #endregion
    }
}
