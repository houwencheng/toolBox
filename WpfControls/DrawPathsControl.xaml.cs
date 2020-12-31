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

        public List<Model.Path> Paths
        {
            get { return (List<Model.Path>)GetValue(PathsProperty); }
            set { SetValue(PathsProperty, value); }
        }


        // Using a DependencyProperty as the backing store for Paths.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathsProperty =
            DependencyProperty.Register("Paths", typeof(List<Model.Path>), typeof(DrawPathsControl), new PropertyMetadata(null, new System.Windows.PropertyChangedCallback(PropertyChangedCallback)));


        public static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DrawPathsControl)d;
            control.RefreshPath();
            control.AddOneAdditionPath();
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
                var control = BuildDrawLineControl(item);
                grid.Children.Add(control);
            }
        }

        private DrawLineControl BuildDrawLineControl(Model.Path item)
        {
            DrawLineControl control = new DrawLineControl();
            control.SetPath(item);
            control.FinishedEvent += ControlAddition_FinishedEvent;
            control.ClearedEvent += ControlAddition_ClearedEvent;
            control.MouseLeftClickEvent += Control_MouseLeftClickEvent;
            return control;
        }

        /// <summary>
        /// 添加一个额外的path
        /// </summary>
        public void AddOneAdditionPath()
        {
            if (Paths == null) Paths = new List<Model.Path>();

            var additionPath = new Model.Path();
            additionPath.Color = DefaultPathColor;
            if (PathNameEnable)
            {
                additionPath.Name = "新区域";
            }


            Paths.Insert(0, additionPath);
            var additionControl = BuildDrawLineControl(additionPath);
            grid.Children.Insert(0, additionControl);
        }

        private void ControlAddition_ClearedEvent(DrawLineControl drawLineControl)
        {
            if (grid.Children.IndexOf(drawLineControl) == 0) return;
            grid.Children.Remove(drawLineControl);
            Paths.Remove(drawLineControl.Path);
        }

        private void ControlAddition_FinishedEvent(DrawLineControl drawLineControl)
        {
            if (!MutiPathEnable && grid.Children.Count > 1)
            {
                grid.Children.RemoveAt(1);
                Paths.RemoveAt(1);
            }

            AddOneAdditionPath();
        }

        private void Control_MouseLeftClickEvent(object sender, MouseButtonEventArgs e)
        {
            DrawLineControl control = (DrawLineControl)((Grid)((Canvas)sender).Parent).Parent;
            var points = control.Points;
            //throw new NotImplementedException();
        }





        /// <summary>
        /// 允许路径名称
        /// </summary>
        public bool PathNameEnable
        {
            get { return (bool)GetValue(PathNameEnableProperty); }
            set { SetValue(PathNameEnableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PathNameEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathNameEnableProperty =
            DependencyProperty.Register("PathNameEnable", typeof(bool), typeof(DrawPathsControl), new PropertyMetadata(true));



        /// <summary>
        /// 路径默认颜色
        /// </summary>
        public Model.Color DefaultPathColor
        {
            get { return (Model.Color)GetValue(DefaultPathColorProperty); }
            set { SetValue(DefaultPathColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultPathColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultPathColorProperty =
            DependencyProperty.Register("DefaultPathColor", typeof(Model.Color), typeof(DrawPathsControl), new PropertyMetadata(new Model.Color { HexRGB = "#ff0000" }));


        /// <summary>
        /// 多路径
        /// </summary>
        public bool MutiPathEnable
        {
            get { return (bool)GetValue(MutiPathEnableProperty); }
            set { SetValue(MutiPathEnableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MutiPathEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MutiPathEnableProperty =
            DependencyProperty.Register("MutiPathEnable", typeof(bool), typeof(DrawPathsControl), new PropertyMetadata(true));


    }
}
