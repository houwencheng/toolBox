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

namespace WpfControls
{
    /// <summary>
    /// DrawPathsControl.xaml 的交互逻辑
    /// </summary>
    public partial class DrawPathsControl : UserControl
    {
        public DrawPathsControl()
        {
            InitializeComponent();
        }

        public List<Models.Path> Paths
        {
            get { return (List<Models.Path>)GetValue(PathsProperty); }
            set { SetValue(PathsProperty, value); }
        }


        // Using a DependencyProperty as the backing store for Paths.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathsProperty =
            DependencyProperty.Register("Paths", typeof(List<Models.Path>), typeof(DrawPathsControl), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(PropertyChangedCallback)));


        public static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DrawPathsControl)d;
            control.RefreshPath();
        }

        /// <summary>
        /// 更新画框
        /// </summary>
        private void RefreshPath()
        {
            grid.Children.Clear();

            var existPath = Paths != null && Paths.Count > 0;
            if (!existPath) return;


            foreach (var item in Paths)
            {
                DrawLineControl control = new DrawLineControl();
                grid.Children.Add(control);
                control.SetPath(item);
            }

        }

    }
}
