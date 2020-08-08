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

namespace TimeSpanSet
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViewModel.TimeSpanSetViewModel TimeSpanSetViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            TimeSpanSetViewModel = new ViewModel.TimeSpanSetViewModel();
            TimeSpanSetViewModel.MaxValue = 24 * 60;
            TimeSpanSetViewModel.UnitValue = 30;
            TimeSpanSetViewModel.UnitLevels = new List<int>() { 1, 2, 4 };
            //TimeSpanSetViewModel.Values = new List<double[]>();
            DataContext = this;
        }
    }
}
