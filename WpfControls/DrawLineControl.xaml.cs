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

namespace WpfControls
{
    /// <summary>
    /// DrawLineControl.xaml 的交互逻辑
    /// </summary>
    public partial class DrawLineControl : UserControl
    {
        public DrawLineControl()
        {
            InitializeComponent();
        }


        private Point _mouseLeftButtonDownPoint;
        private Point _mouseLeftButtonUpPoint;
        private bool _isMouseLeftClick;
        private List<Point> _points = new List<Point>();
        /// <summary>
        /// 鼠标单击委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void MouseLeftClickHandel(object sender, MouseButtonEventArgs e);
        /// <summary>
        /// 鼠标单击事件
        /// </summary>
        public event MouseLeftClickHandel MouseLeftClickEvent;

        /// <summary>
        /// 正在画的线
        /// </summary>
        private Line drawingLine;
        private Line selectedLine;

        public bool Finished { get { return CheckDrawValidate(); } }

        #region event method

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Reset();
            var canvas = (Canvas)sender;
            _mouseLeftButtonDownPoint = e.GetPosition(canvas);

            BeginDrawNewLine(canvas);
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = (Canvas)sender;
            ClearDraw(canvas);
            FinishDraw();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var canvas = (Canvas)sender;
            var mousePoint = e.GetPosition(canvas);
            DrawLineing(mousePoint);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var canvas = (Canvas)sender;
            _mouseLeftButtonUpPoint = e.GetPosition(canvas);

            CheckTrigerMouseClickEvent(sender, e);
            if (Finished) return;
            EndDrawNewLine(canvas);
        }


        private void Canvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                // delete line
            }
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReFreshDraw();
        }

        #endregion

        #region private method

        /// <summary>
        /// 判断两点是否相近
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private static bool IsCloserPoint(Point point1, Point point2)
        {
            var closer = 10;
            var x1 = point1.X;
            var y1 = point1.Y;
            var x2 = point2.X;
            var y2 = point2.Y;
            var isCloserPoint = Math.Abs(x2 - x1) < closer && Math.Abs(y2 - y1) < closer;
            return isCloserPoint;
        }

        /// <summary>
        /// 判断并触发单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckTrigerMouseClickEvent(object sender, MouseButtonEventArgs e)
        {
            var point1 = _mouseLeftButtonDownPoint;
            var point2 = _mouseLeftButtonUpPoint;

            bool isCloserPoint = IsCloserPoint(point1, point2);
            if (isCloserPoint) _isMouseLeftClick = true;
            else _isMouseLeftClick = false;

            if (_isMouseLeftClick)
                if (MouseLeftClickEvent != null)
                    MouseLeftClickEvent(sender, e);
        }

        /// <summary>
        /// 开始画新线
        /// </summary>
        /// <param name="canvas"></param>
        private void BeginDrawNewLine(Canvas canvas)
        {
            if (Finished) return;

            var count = canvas.Children.Count;
            var isContinueAdd = count > 0;
            var point = _mouseLeftButtonDownPoint;
            if (isContinueAdd)
            {
                var lastLine = (Line)canvas.Children[count - 1];
                var point2 = new Point(lastLine.X2, lastLine.Y2);
                var isCloserPoint = IsCloserPoint(point, point2);
                if (!isCloserPoint) return;
                point = point2;
            }

            drawingLine = NewLine();
            drawingLine.X1 = point.X;
            drawingLine.Y1 = point.Y;
            drawingLine.X2 = point.X;
            drawingLine.Y2 = point.Y;
            canvas.Children.Add(drawingLine);
        }

        /// <summary>
        /// 检查画的线，如果两点很近则取消掉画线
        /// </summary>
        /// <param name="canvas"></param>
        private void EndDrawNewLine(Canvas canvas)
        {
            _points = GetPathPointList();
            var point1 = _mouseLeftButtonDownPoint;
            var point2 = _mouseLeftButtonUpPoint;
            bool isCloserPoint = IsCloserPoint(point1, point2);
            if (isCloserPoint && drawingLine != null)
            {
                canvas.Children.Remove(drawingLine);
                _points = GetPathPointList();
                return;
            }

            var count = canvas.Children.Count;
            var isContinueAdd = count > 0;
            if (isContinueAdd)
            {
                var firstLine = (Line)canvas.Children[0];
                var pointBegin = new Point(firstLine.X1, firstLine.Y1);
                var isCloserBegin = IsCloserPoint(point2, pointBegin);
                if (isCloserBegin)
                {
                    var lastLine = (Line)canvas.Children[count - 1];
                    lastLine.X2 = firstLine.X1;
                    lastLine.Y2 = firstLine.Y1;
                    _points = GetPathPointList();
                    if (Finished)
                    {
                        FinishDraw();
                        OnFinishedEvent();
                    }
                }
            }

        }

        /// <summary>
        /// 完成绘制
        /// </summary>
        private void FinishDraw()
        {
            _points = GetPathPointList();
            //Points.Clear();
            //Points.AddRange(_points);
            Points = _points.Select(x => new Model.Point { X = x.X, Y = x.Y }).ToList();
            //Finished = true;
            //BuildPath();
        }


        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="mousePoint"></param>
        private void DrawLineing(Point mousePoint)
        {
            var leftButtonPressed = Mouse.LeftButton == MouseButtonState.Pressed;
            if (leftButtonPressed && drawingLine != null)
            {
                drawingLine.X2 = mousePoint.X;
                drawingLine.Y2 = mousePoint.Y;
            }
        }

        /// <summary>
        /// 重置参数
        /// </summary>
        private void Reset()
        {
            _mouseLeftButtonDownPoint = new Point();
            _mouseLeftButtonUpPoint = new Point();
            drawingLine = null;
        }

        /// <summary>
        /// 清除
        /// </summary>
        /// <param name="canvas"></param>
        private void ClearDraw(Canvas canvas)
        {
            _points.Clear();
            ReFreshDraw();
            //Finished = false;
            OnClearedEvent();
        }

        /// <summary>
        /// 返回指定样式的Line
        /// </summary>
        /// <returns></returns>
        private Line NewLine()
        {
            var newLine = new Line();
            var lineStyle = this.FindResource("lineNormol");
            newLine.SetValue(StyleProperty, lineStyle);
            if (Path != null)
                newLine.Stroke = Path.Level.Color.ToWindowsBrush();
            return newLine;
        }

        /// <summary>
        /// 获取点列表
        /// </summary>
        /// <returns></returns>
        private List<Point> GetPathPointList()
        {
            var points = new List<Point>();
            var width = canvas.ActualWidth;
            var height = canvas.ActualHeight;
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                var line = (Line)canvas.Children[i];
                var beginPoint = new Point(line.X1 / width, line.Y1 / height);
                var endPoint = new Point(line.X2 / width, line.Y2 / height);
                points.Add(beginPoint);
                var isLastLine = i == canvas.Children.Count - 1;
                if (isLastLine)
                {
                    points.Add(endPoint);
                }
            }

            return points;
        }

        /// <summary>
        /// 获取点列表
        /// </summary>
        /// <returns></returns>
        private void BuildPath()
        {
            var geometryString = string.Empty;
            var width = canvas.ActualWidth;
            var height = canvas.ActualHeight;
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                var line = (Line)canvas.Children[i];
                var firstLine = i == 0;
                var isLastLine = i == canvas.Children.Count - 1;
                var beginPoint = new Point(line.X1, line.Y1);
                var endPoint = new Point(line.X2, line.Y2);
                if (firstLine)
                {
                    geometryString = string.Format("M{0},{1}", line.X1, line.Y1);
                }

                geometryString = string.Format("{0} L{1},{2}", geometryString, line.X2, line.Y2);
            }

            Path path = new Path();
            path.Style = (Style)this.FindResource("pathStyle");
            if (Path != null)
                path.Fill = Path.Level.Color.ToWindowsBrush();
            GeometryConverter geometryConverter = new GeometryConverter();
            path.Data = (Geometry)geometryConverter.ConvertFrom(geometryString);
            path.Height = height;
            path.Width = width;
            //canvas.Children.Clear();
            canvas.Children.Add(path);
        }

        /// <summary>
        /// 设置点列表
        /// </summary>
        /// <returns></returns>
        private void SetPathPointList(List<Model.Point> points)
        {
            _points = points.Select(x => new System.Windows.Point() { X = x.X, Y = x.Y }).ToList();
            ReFreshDraw();
        }

        private bool CheckDrawValidate()
        {
            // 点数检查
            var pointCountValidate = _points.Count > 3;
            if (!pointCountValidate) return false;

            //首尾是否闭合
            var closed = false;
            var firstPoint = _points.First();
            var lastPoint = _points.Last();
            closed = firstPoint.Equals(lastPoint);
            if (!closed) return false;

            // 是否在一条直线
            var allOnLine = true;
            double range = 0.02;
            double d = 0;
            for (int i = 1; i < _points.Count - 1; i++)
            {
                var point = _points[i];
                var diffX = point.X - firstPoint.X;
                var diffY = point.Y - firstPoint.Y;
                var diff = diffX / diffY;
                if (d == 0)
                {
                    d = diff;
                    continue;
                }

                var onLine = Math.Abs(diff / d - 1) <= range;
                if (onLine) continue;
                else { allOnLine = false; break; }
            }

            if (allOnLine) return false;

            // 边检查

            return true;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private void ReFreshDraw()
        {
            canvas.Children.Clear();
            var width = canvas.ActualWidth;
            var height = canvas.ActualHeight;
            for (int i = 0; i < _points.Count - 1; i++)
            {
                var beginPoint = _points[i];
                var endPoint = _points[i + 1];
                Line drawingLine = NewLine();
                drawingLine.X1 = beginPoint.X * width;
                drawingLine.Y1 = beginPoint.Y * height;
                drawingLine.X2 = endPoint.X * width;
                drawingLine.Y2 = endPoint.Y * height;
                canvas.Children.Add(drawingLine);
            }

            if (Finished)
            {
                BuildPath();
                UpdateText();
            }

        }

        /// <summary>
        /// 触发完成事件
        /// </summary>
        private void OnFinishedEvent()
        {
            canvas.Background = null;
            if (FinishedEvent != null)
                FinishedEvent(this);
        }

        /// <summary>
        /// 更新区域文本框
        /// </summary>
        private void UpdateText()
        {
            if (Points == null)
            {
                tb.Visibility = Visibility.Hidden;
                return;
            }

            tb.Visibility = Visibility.Visible;

            var leftMin = 1.0;
            var leftMax = 0.0;
            var topMin = 1.0;
            var topMax = 0.0;

            Points.ForEach(x =>
            {
                if (x.X < leftMin) leftMin = x.X;
                if (x.X > leftMax) leftMax = x.X;

                if (x.Y < topMin) topMin = x.Y;
                if (x.Y > topMax) topMax = x.Y;
            });

            var left = 0.0;
            var top = 0.0;
            left = (leftMin + leftMax) / 2;
            top = (topMin + topMax) / 2;
            tb.Margin = new Thickness { Left = left * canvas.ActualWidth, Top = top * canvas.ActualHeight };
        }

        /// <summary>
        /// 触发清除事件
        /// </summary>
        private void OnClearedEvent()
        {
            if (ClearedEvent != null)
                ClearedEvent(this);
        }

        #endregion


        public List<Model.Point> Points
        {
            get { return (List<Model.Point>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Points.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(List<Model.Point>), typeof(DrawLineControl), new PropertyMetadata(null, new PropertyChangedCallback(PointsChangedCallback)));


        private static void PointsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DrawLineControl)d;
            if (e.NewValue == null)
            {
                control.ClearDraw(control.canvas);
            }
            else
            {
                var points = (List<Model.Point>)e.NewValue;
                control.SetPathPointList(points);
            }
        }

        public Model.Path Path { get; set; }

        public void SetPath(Model.Path path)
        {
            Path = path;
            System.Windows.Data.Binding binding = new Binding();
            binding.Source = Path;
            binding.Path = new PropertyPath("Points");
            binding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(this, DrawLineControl.PointsProperty, binding);

            System.Windows.Data.Binding binding2 = new Binding();
            binding2.Source = Path;
            binding2.Path = new PropertyPath("Level.Name");
            binding2.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(tb, EditableTextBlack.TextProperty, binding2);

            if (path.Points == null)
                canvas.Background = new SolidColorBrush(System.Windows.Media.Colors.Transparent);
        }

        public event Action<DrawLineControl> FinishedEvent;
        public event Action<DrawLineControl> ClearedEvent;
    }
}
