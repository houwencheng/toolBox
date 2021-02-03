using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class PlayTimeLine : UserControl, System.ComponentModel.INotifyPropertyChanged
    {
        public PlayTimeLine()
        {
            InitializeComponent();
            canvas.SizeChanged += Canvas_SizeChanged;
            grid.MouseMove += Grid_MouseMove;
            grid.MouseWheel += Grid_MouseWheel;
            grid.SizeChanged += Grid_SizeChanged;
            grid.MouseLeftButtonDown += Grid_MouseLeftButtonDown;
            grid.MouseLeftButtonUp += Grid_MouseLeftButtonUp;

            //SimuPlay();
            playBorder.MouseEnter += PlayBorder_MouseEnter;
            playBorder.MouseMove += PlayBorder_MouseMove;
            playBorder.MouseLeave += PlayBorder_MouseLeave;
            playBorder.MouseLeftButtonDown += PlayBorder_MouseLeftButtonDown;
            playBorder.MouseLeftButtonUp += PlayBorder_MouseLeftButtonUp;
            playBorder.DataContext = this;
        }

        private bool isDragGrid = false;

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragGrid = true;
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragGrid = false;
        }

        private void PlayBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragPlay)
            {
                _playPosition = _playPosition2;
                playBorder.Cursor = Cursors.Arrow;
                e.Handled = true;
                if (PlayUnixTimestamp != _playPosition)
                    PlayUnixTimestamp = _playPosition;
            }

            isDragPlay = false;
        }

        private bool isDragPlay = false;

        private void PlayBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragPlay = true;
            _mousePosition2 = e.GetPosition(this);
            _playPosition2 = _playPosition;
            playBorder.Cursor = Cursors.SizeWE;
            e.Handled = true;
        }

        private void PlayBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            playMarkAllowMove = true;
            isDragPlay = false;
        }

        private void PlayBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            playMarkAllowMove = false;
        }

        private bool playMarkAllowMove = true;
        private string timeStringFormat = "yyyy/MM/dd HH:mm:ss";

        private Point _mousePosition2;
        private double _playPosition2;
        private void PlayBorder_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            var mousePosition = e.GetPosition(this);
            if (isDragPlay && e.LeftButton == MouseButtonState.Pressed)
            {
                var leftPixes = (mousePosition - _mousePosition2).X;
                if (leftPixes == 0) return;
                _playPosition2 += leftPixes / ratioPixesWithValue;
                RefreshPlayMark(_playPosition2 - leftValue);
                _mousePosition2 = mousePosition;
            }

        }

        /// <summary>
        /// 左值 Unix时间戳
        /// </summary>
        private double leftValue;

        /// <summary>
        /// 播放位置(Unix时间戳)
        /// </summary>
        double _playPosition;

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePosition = e.GetPosition(this);
            if (isDragGrid && e.LeftButton == MouseButtonState.Pressed)
            {
                var originLeft = (double)grid.GetValue(System.Windows.Controls.Canvas.LeftProperty);
                if (double.IsNaN(originLeft)) originLeft = 0;
                var nowLeft = originLeft + (mousePosition - _mousePosition).X;
                grid.SetValue(System.Windows.Controls.Canvas.LeftProperty, nowLeft);
                CheckGridBound(nowLeft);
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
            ReBuild();
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var tempIndex = _markSpanPixesIndex;
            if (e.Delta < 0)
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

            if (tempIndex != _markSpanPixesIndex)
            {
                var mousePosition = e.GetPosition(this);
                var mousePositionValue = leftValue + mousePosition.X * _precisionList[tempIndex] / _markSpanPixesList[tempIndex];
                leftValue = mousePositionValue - mousePosition.X * _precisionList[_markSpanPixesIndex] / _markSpanPixesList[_markSpanPixesIndex];

                ReBuild();

            }

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

            leftValue -= lefterWidth / ratioPixesWithValue;

            ReBuild();
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
            leftValue += righterWidth / ratioPixesWithValue;
            ReBuild();
        }

        #endregion

        #region 刻度

        /// <summary>
        /// 相邻等级刻度高度等比
        /// </summary>
        private double _levelHeightRatio = 1.3;
        /// <summary>
        /// 调刻度相距上边的距离
        /// </summary>
        private double _levelMarginTop = 2;

        /// <summary>
        /// 精度列表单位秒
        /// </summary>
        List<double> _precisionList = new List<double> {
            1,
            30,
            60,
            5*60,
            10*60,
            30*60,
            60*60,
            2*60*60,
            6*60*60,
            12*60*60,
            24*60*60,
            7*24*60*60,
            30*24*60*60,
            12*30*24*60*60};
        List<string> _precisionListName = new List<string> {
            "1秒",
            "30秒",
            "1分",
            "5分",
            "10分",
            "30分",
            "1小时",
            "2小时",
            "6小时",
            "12小时",
            "1天",
            "1周",
            "约1月",
            "约1年"};
        /// <summary>
        /// 单位刻度像素宽度列表
        /// </summary>
        List<double> _markSpanPixesList = new List<double> { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        /// <summary>
        /// 使用的刻度下标
        /// </summary>
        int _markSpanPixesIndex = 0;

        /// <summary>
        /// 等级列表
        /// </summary>
        private List<double> _levelList = new List<double> { 1, 2, 4, 8, 16 };

        /// <summary>
        /// 总的刻度数
        /// </summary>
        private int markCount;

        /// <summary>
        /// 像素和值的比率
        /// </summary>
        private double ratioPixesWithValue
        {
            get
            {
                var unitValue = _precisionList[_markSpanPixesIndex];
                var unitPixes = _markSpanPixesList[_markSpanPixesIndex];
                var ratio = unitPixes / unitValue;
                return ratio;
            }
        }



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
        /// 重新生成刻度和录像记录
        /// </summary>
        private void ReBuild()
        {
            // 刻度线
            BuildMark();
            // 记录线
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
                path.Stroke = this.Foreground;
                var markValue = leftValue + _precisionList[_markSpanPixesIndex] * i;
                var markDateTime = baseTime.AddSeconds(markValue).ToLocalTime();
                path.DataContext = markDateTime;
                path.ToolTip = string.Format("{1}", i, markDateTime.ToString(timeStringFormat));
                markGrid.Children.Add(path);
            }

            UpdateMarkUnitTxt();
        }

        /// <summary>
        /// 更新刻度单位提示
        /// </summary>
        private void UpdateMarkUnitTxt()
        {
            var text = _precisionListName[_markSpanPixesIndex];
            unitTxt.Text = text;
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
            AutoAjustRecordTimespanToView();
            ReBuild();
        }

        /// <summary>
        /// 自动调整录像时段到可视区域
        /// </summary>
        private void AutoAjustRecordTimespanToView()
        {
            double minValue = 0;
            double maxValue = 0;
            for (int i = 0; i < _recordTimeSpan.Count; i++)
            {
                var record = _recordTimeSpan[i];
                var begin = record[0];
                var end = record[1];

                if (i == 0)
                {
                    minValue = begin;
                    maxValue = end;
                    continue;
                }

                if (minValue > begin) minValue = begin;
                if (maxValue < end) maxValue = end;
            }

            leftValue = minValue;

            var maxDiff = maxValue - minValue;
            var maxWidth = recordGrid.ActualWidth;

            for (int i = 0; i < _markSpanPixesList.Count; i++)
            {
                _markSpanPixesIndex = i;
                var unitPixes = _markSpanPixesList[_markSpanPixesIndex];
                var unitValue = _precisionList[_markSpanPixesIndex];
                if (maxWidth > maxDiff / unitValue * unitPixes)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 生成录像刻度
        /// </summary>
        private void BuildVieoRecord()
        {
            recordGrid.Children.Clear();

            var x = 0d;
            var width = 30d;
            var y = 0d;
            var height = recordGrid.ActualHeight;
            var maxWidth = recordGrid.ActualWidth;

            for (int i = 0; i < _recordTimeSpan.Count; i++)
            {
                var record = _recordTimeSpan[i];
                var begin = record[0];
                x = (begin - leftValue) * ratioPixesWithValue;

                var end = record[1];
                width = (end - begin) * ratioPixesWithValue;

                if (x + width < 0) continue;
                if (x > maxWidth) continue;
                if (width <= 0) continue;

                System.Windows.Media.RectangleGeometry rectangleGeometry = new RectangleGeometry();
                rectangleGeometry.Rect = new Rect { X = x, Y = y, Height = height, Width = width };

                Path path = new Path();
                path.Fill = Brushes.Green;
                path.Data = rectangleGeometry;

                var beginDateTime = baseTime.AddSeconds(begin).ToLocalTime();
                var endDateTime = baseTime.AddSeconds(end).ToLocalTime();
                var format = "{1}到{2}";

                path.DataContext = record;
                path.ToolTip = string.Format(format, i, beginDateTime.ToString(timeStringFormat), endDateTime.ToString(timeStringFormat));
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
                    _playPosition++;
                    Dispatcher.Invoke(() =>
                    {
                        var unitPixes = _markSpanPixesList[_markSpanPixesIndex];
                        var left = (_playPosition - leftValue) * ratioPixesWithValue;

                        if (playMarkAllowMove)
                        {
                            var gridLeft = (double)grid.GetValue(Canvas.LeftProperty);
                            if (double.IsNaN(gridLeft)) gridLeft = 0;
                            var hideLeft = left + gridLeft < 0;
                            // 右预留20个单元
                            var data = left + gridLeft - canvas.ActualWidth + unitPixes * 20;
                            var hideRight = data > 0;
                            if (hideRight)
                            {
                                var newCanvasLeft = gridLeft - data;
                                grid.SetValue(Canvas.LeftProperty, newCanvasLeft);
                                CheckGridBound(newCanvasLeft);
                            }

                            RefreshPlayMark(_playPosition - leftValue);
                        }
                    });
                }
            });
        }


        private string playDateTime;
        public string PlayDateTime
        {
            get => playDateTime; set
            {
                DateTime dt;
                var state = DateTime.TryParse(value, out dt);
                if (state)
                {
                    playDateTime = value;
                    PlayUnixTimestamp = (dt.ToUniversalTime() - baseTime).TotalSeconds;
                }
            }
        }

        /// <summary>
        /// 是否在编辑时间
        /// </summary>
        public bool IsEditPlayDateTime { get; set; }


        /// <summary>
        /// 刷新播放刻度
        /// </summary>
        /// <param name="left"></param>
        private void RefreshPlayMark(double playPosition)
        {
            var leftPixes = playPosition * ratioPixesWithValue;
            playDateTime = baseTime.AddSeconds(playPosition + leftValue).ToLocalTime().ToString(timeStringFormat);

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs("PlayDateTime"));

            playBorder.Visibility = Visibility.Visible;
            playBorder.Margin = new Thickness { Left = leftPixes };
        }

        /// <summary>
        /// 检查grid边界
        /// </summary>
        /// <param name="newCanvasLeft"></param>
        private void CheckGridBound(double newCanvasLeft)
        {
            if (newCanvasLeft > 0) LoadLefter();
            else if (newCanvasLeft + grid.ActualWidth < canvas.ActualWidth) LoadRighter();
        }


        #endregion


        private void ChangedPlayPosition()
        {
            _playPosition = PlayUnixTimestamp;

            var unitPixes = _markSpanPixesList[_markSpanPixesIndex];
            // 左，右预留20个单元
            var playMargin = unitPixes * 20;
            var playMarginValue = playMargin / ratioPixesWithValue;

            if (_playPosition < leftValue)
            {
                leftValue = _playPosition - playMarginValue;
                grid.SetValue(Canvas.LeftProperty, 0d);
            }
            else if (_playPosition > leftValue + grid.ActualWidth / ratioPixesWithValue)
            {
                leftValue = _playPosition - grid.ActualWidth / ratioPixesWithValue + playMarginValue;
                grid.SetValue(Canvas.LeftProperty, grid.ActualWidth - canvas.ActualWidth);
            }


            if (playMarkAllowMove
                && !IsEditPlayDateTime
                && !isDragPlay)
            {
                while (true)
                {
                    var left = (_playPosition - leftValue) * ratioPixesWithValue;
                    var gridLeft = (double)grid.GetValue(Canvas.LeftProperty);
                    if (double.IsNaN(gridLeft)) gridLeft = 0;
                    var hideLeft = left + gridLeft < 0;
                    if (hideLeft)
                    {
                    }


                    var data = left + gridLeft - canvas.ActualWidth + playMargin;
                    var hideRight = data > 0;
                    if (hideRight)
                    {
                        var newCanvasLeft = gridLeft - data;
                        grid.SetValue(Canvas.LeftProperty, newCanvasLeft);
                        CheckGridBound(newCanvasLeft);
                    }
                    else break;
                }

                RefreshPlayMark(_playPosition - leftValue);
            }
        }

        private void ChangedRecordTimeSpanList()
        {
            SetRecordTimespan(RecordTimeSpanList);
        }

        /// <summary>
        /// 设置播放的Unix时间戳
        /// </summary>
        public double PlayUnixTimestamp
        {
            get { return (double)GetValue(PlayUnixTimestampProperty); }
            set { SetValue(PlayUnixTimestampProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlayUnixTimeStamp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayUnixTimestampProperty =
            DependencyProperty.Register("PlayUnixTimestamp", typeof(double), typeof(PlayTimeLine), new PropertyMetadata(0d, PlayUnixTimeStampPropertyChangedCallback));


        public static void PlayUnixTimeStampPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PlayTimeLine)d;
            control.ChangedPlayPosition();
        }




        public List<double[]> RecordTimeSpanList
        {
            get { return (List<double[]>)GetValue(RecordTimeSpanListProperty); }
            set { SetValue(RecordTimeSpanListProperty, value); }
        }


        // Using a DependencyProperty as the backing store for RecordTimeSpanList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecordTimeSpanListProperty =
            DependencyProperty.Register("RecordTimeSpanList", typeof(List<double[]>), typeof(PlayTimeLine), new PropertyMetadata(null, RecordTimeSpanListPropertyChangedCallback));

        public event PropertyChangedEventHandler PropertyChanged;

        public static void RecordTimeSpanListPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PlayTimeLine)d;
            control.ChangedRecordTimeSpanList();
        }


    }
}
