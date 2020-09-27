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
    /// PlayTimeLine.xaml 的交互逻辑
    /// *Mark是刻度的意思
    /// </summary>
    public partial class PlayTimeLine : UserControl
    {
        public PlayTimeLine()
        {
            InitializeComponent();
            canvas.SizeChanged += Canvas_SizeChanged;
            grid.MouseMove += Grid_MouseMove;
            grid.MouseWheel += Grid_MouseWheel;
            grid.SizeChanged += Grid_SizeChanged;

            leftValue = (DateTime.Now.Date.AddDays(-1) - baseTime).TotalSeconds;
            playPosition = leftValue;
            UpdateMarkUnitTxt();

            SimuPlay();
        }

        /// <summary>
        /// 左值 Unix时间戳
        /// </summary>
        private double leftValue;

        /// <summary>
        /// 播放位置(Unix时间戳)
        /// </summary>
        double playPosition;

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(this);
            var mouseLeftButtonDown = e.LeftButton == MouseButtonState.Pressed;
            if (mouseLeftButtonDown)
            {
                var originLeft = (double)grid.GetValue(System.Windows.Controls.Canvas.LeftProperty);
                if (double.IsNaN(originLeft)) originLeft = 0;
                var nowLeft = originLeft + (mousePosition - _mousePosition).X;
                grid.SetValue(System.Windows.Controls.Canvas.LeftProperty, nowLeft);

                if (nowLeft > 0) LoadLefter();
                else if (nowLeft + grid.ActualWidth < canvas.ActualWidth) LoadRighter();
            }

            _mousePosition = mousePosition;
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var ratio = e.NewSize.Width / e.PreviousSize.Width;
            var originLeft = (double)grid.GetValue(System.Windows.Controls.Canvas.LeftProperty);
            var nowLeft = originLeft * ratio;
            grid.SetValue(System.Windows.Controls.Canvas.LeftProperty, nowLeft);
            grid.Width = canvas.ActualWidth * _ratio;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Refresh();
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var tempIndex = _markSpanPixesIndex;
            if (e.Delta > 0)
            {
                if (_markSpanPixesIndex < _markSpanPixesList.Count - 1)
                {
                    _markSpanPixesIndex++;
                }
            }
            else
            {
                if (_markSpanPixesIndex > 0)
                {
                    _markSpanPixesIndex--;
                }
            }

            UpdateMarkUnitTxt();
            if (tempIndex != _markSpanPixesIndex)
                Refresh();

        }

        /// <summary>
        /// 基址时间
        /// </summary>
        private DateTime baseTime = DateTime.Parse("1970/01/01");

        #region 时间线和容器控制

        /// <summary>
        /// 时间线和外容器的长度比率，应大于1
        /// </summary>
        private double _ratio = 2;

        /// <summary>
        /// 拖拽世间条时鼠标相对于控件的位置
        /// </summary>
        private Point _mousePosition;


        /// <summary>
        /// 加载左边的时间条
        /// </summary>
        private void LoadLefter()
        {
            var widthDiff = grid.ActualWidth - canvas.ActualWidth;
            // 相左加载的宽度
            var lefterWidth = widthDiff / 2;
            grid.SetValue(Canvas.LeftProperty, -lefterWidth);

            var unitValue = _precisionList[_markSpanPixesIndex];
            var unitPixes = _markSpanPixesList[_markSpanPixesIndex];
            var ratio = unitValue / unitPixes;
            leftValue -= lefterWidth * ratio;

            Refresh();
        }

        /// <summary>
        /// 加载右边的时间条
        /// </summary>
        private void LoadRighter()
        {
            var widthDiff = grid.ActualWidth - canvas.ActualWidth;
            // 相右加载的宽度
            var righterWidth = widthDiff / 2;
            grid.SetValue(Canvas.LeftProperty, -righterWidth);
            var unitValue = _precisionList[_markSpanPixesIndex];
            var unitPixes = _markSpanPixesList[_markSpanPixesIndex];
            var ratio = unitValue / unitPixes;
            leftValue += righterWidth * ratio;
            Refresh();
        }

        #endregion

        #region 刻度

        /// <summary>
        /// 相邻等级刻度高度等比
        /// </summary>
        private double _levelHeightRatio = 1.5;
        /// <summary>
        /// 调刻度相距上边的距离
        /// </summary>
        private double _levelMarginTop = 2;

        /// <summary>
        /// 精度列表单位秒
        /// </summary>
        List<double> _precisionList = new List<double> {
            1,
            60,
            5 * 60,
            10 * 60,
            30 * 60,
            60 * 60,
            2* 60 * 60};
        /// <summary>
        /// 单位刻度像素宽度列表
        /// </summary>
        List<double> _markSpanPixesList = new List<double> { 3, 5, 7, 9, 10, 13 };

        /// <summary>
        /// 使用的刻度下标
        /// </summary>
        int _markSpanPixesIndex = 2;

        /// <summary>
        /// 等级列表
        /// </summary>
        private List<double> _levelList = new List<double> { 1, 2, 4, 8 };

        /// <summary>
        /// 总的刻度数
        /// </summary>
        private int markCount;



        /// <summary>
        /// 获取等级刻度的高度
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private double GetLevelHeight(int level)
        {
            var maxHeight = markGrid.ActualHeight - _levelMarginTop;
            double theLevelHeight = RecGetLevelHeight(level, maxHeight);
            return theLevelHeight;
        }

        private double RecGetLevelHeight(int level, double LevelHeight)
        {
            if (level == 1) return LevelHeight;
            var nextLevel = --level;
            var nextLevelHeight = LevelHeight / _levelHeightRatio;
            return RecGetLevelHeight(nextLevel, nextLevelHeight);
        }


        /// <summary>
        /// 生成刻度
        /// </summary>
        private void Refresh()
        {
            BuildMark();
            if (_recordTimeSpan != null && _recordTimeSpan.Count > 0)
                BuildVieoRecord();
        }

        private void BuildMark()
        {
            markGrid.Children.Clear();
            var markSpanPixes = _markSpanPixesList[_markSpanPixesIndex];
            markCount = (int)(markGrid.ActualWidth / markSpanPixes) + 1;
            for (int i = 0; i < markCount; i++)
            {
                Point startPoint = new Point { X = i * markSpanPixes, Y = markGrid.ActualHeight };
                Point endPoint = new Point();
                endPoint.X = startPoint.X;
                for (int j = _levelList.Count - 1; j >= 0; j--)
                {
                    var unitCount = _levelList[j];
                    var isTheLevel = i % unitCount == 0;
                    if (isTheLevel)
                    {
                        var level = _levelList.Count - j;
                        var levelHeight = GetLevelHeight(level);
                        endPoint.Y = startPoint.Y - levelHeight;
                        break;
                    }
                }

                LineGeometry lineGeometry = new LineGeometry();
                lineGeometry.StartPoint = startPoint;
                lineGeometry.EndPoint = endPoint;

                Path path = new Path();
                path.StrokeThickness = 1;
                path.Data = lineGeometry;
                path.Stroke = Brushes.Black;
                var markValue = leftValue + _precisionList[_markSpanPixesIndex] * i;
                var markDateTime = baseTime.AddSeconds(markValue);
                path.DataContext = markDateTime;
                path.ToolTip = new TextBlock { Text = string.Format("Num.{0}@{1}", i, markDateTime), Foreground = path.Stroke };
                markGrid.Children.Add(path);
            }
        }

        /// <summary>
        /// 更新刻度单位提示
        /// </summary>
        private void UpdateMarkUnitTxt()
        {
            unitTxt.Text = string.Format("{0}秒", _precisionList[_markSpanPixesIndex]);
        }

        #endregion

        #region 录像记录
        /// <summary>
        /// 录像时间段数据
        /// </summary>
        List<double[]> _recordTimeSpan;

        /// <summary>
        /// 设置录像数据
        /// </summary>
        /// <param name="timeSpan"></param>
        public void SetRecordTimespan(List<double[]> timeSpan)
        {
            _recordTimeSpan = timeSpan;
            BuildVieoRecord();
        }

        /// <summary>
        /// 生成录像刻度
        /// </summary>
        public void BuildVieoRecord()
        {
            recordGrid.Children.Clear();

            var x = 0d;
            var width = 30d;
            var y = 0d;
            var height = recordGrid.ActualHeight;
            var maxWidth = recordGrid.ActualWidth;

            var unitValue = _precisionList[_markSpanPixesIndex];
            var unitPixes = _markSpanPixesList[_markSpanPixesIndex];
            var ratio = unitPixes / unitValue;
            for (int i = 0; i < _recordTimeSpan.Count; i++)
            {
                var record = _recordTimeSpan[i];
                var begin = record[0];
                x = (begin - leftValue) * ratio;

                var end = record[1];
                if (end > begin)
                {
                    width = (end - begin) * ratio;
                }
                else
                {
                    width = maxWidth - x;
                }

                if (x + width < 0) continue;
                if (x > maxWidth) continue;

                System.Windows.Media.RectangleGeometry rectangleGeometry = new RectangleGeometry();
                rectangleGeometry.Rect = new Rect { X = x, Y = y, Height = height, Width = width };

                Path path = new Path();
                path.Fill = Brushes.Green;
                path.Data = rectangleGeometry;
                recordGrid.Children.Add(path);
            }
        }

        #endregion

        #region 播放刻度

        /// <summary>
        /// 模拟播放
        /// </summary>
        public void SimuPlay()
        {
            Task.Run(() =>
            {
                var times = 1;
                while (times++ < 60 * 60)
                {
                    System.Threading.Thread.Sleep(1000);
                    playPosition++;
                    Dispatcher.Invoke(() =>
                    {
                        var left = times * _markSpanPixesList[_markSpanPixesIndex] / _precisionList[_markSpanPixesIndex];
                        playBorder.Margin = new Thickness { Left = left };
                        var playDateTime = baseTime.AddSeconds(playPosition);
                        playBorder.DataContext = playDateTime;
                        playBorder.ToolTip = new TextBlock { Text = string.Format("Num.{0}@{1}", times, playDateTime), Foreground = playBorder.Background };
                    });
                }
            });
        }
        #endregion
    }
}
