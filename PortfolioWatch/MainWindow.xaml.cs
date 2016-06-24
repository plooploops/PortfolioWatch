using PortfolioWatch.ViewModels;
using PortfolioWatchBL.Helper;
using PortfolioWatchBO.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PortfolioWatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ParentViewModel _parentViewModel;

        public MainWindow()
        {
            InitializeComponent();

            UpdateParentViewModel();

            UpdateDataContext();
        }

        private void UpdateParentViewModel()
        {
            // messagebox
            var popup = (Action<string>)(msg => MessageBox.Show(msg));

            // confirm box
            var confirm = (Func<string, string, bool>)((msg, capt) =>
                MessageBox.Show(msg, capt, MessageBoxButton.YesNo) == MessageBoxResult.Yes);
            _parentViewModel = new ParentViewModel(popup, confirm);

        }

        public void UpdateDataContext()
        {
            this.DataContext = _parentViewModel;
        }

    }
}
