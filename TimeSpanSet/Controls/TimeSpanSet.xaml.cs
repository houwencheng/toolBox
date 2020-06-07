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

namespace TimeSpanSet.Controls
{
    /// <summary>
    /// TimeSpanSet.xaml 的交互逻辑
    /// </summary>
    public partial class TimeSpanSet : UserControl
    {
        public TimeSpanSet()
        {
            InitializeComponent();
            canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseLeave += Canvas_MouseLeave;
            canvas.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            canvas.SizeChanged += TimeSpanSet_SizeChanged;
        }

        private void TimeSpanSet_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var oldWidth = e.PreviousSize.Width;
            var newWidth = e.NewSize.Width;
            // 比率
            var ratio = newWidth / oldWidth;
            if (double.IsInfinity(ratio)) return;

            Ratio = Ratio * ratio;

            ScalarSpan(ratio);
            ScalarMark(ratio);
        }

        /// <summary>
        /// 缩放标尺
        /// </summary>
        /// <param name="ratio"></param>
        private void ScalarMark(double ratio)
        {
            for (int i = 0; i < markCanvas.Children.Count; i++)
            {
                var mark = (Mark)markCanvas.Children[i];
                var left = (double)mark.GetValue(Canvas.LeftProperty);
                var newLeft = left * ratio;
                mark.SetValue(Canvas.LeftProperty, newLeft);
            }
        }

        /// <summary>
        /// 伸缩条
        /// </summary>
        /// <param name="ratio"></param>
        private void ScalarSpan(double ratio)
        {
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                var border = (Span)canvas.Children[i];
                var width = border.Width;
                var newWidth2 = ratio * width;

                var left = (double)border.GetValue(Canvas.LeftProperty);
                var newLeft = left * ratio;

                border.SetValue(Canvas.LeftProperty, newLeft);
                border.Width = newWidth2;
            }
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            EndNewSpan();
            EndDrag();
            EndSizeRight();
            EndSizeLeft();
            UpdateToSource();
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EndNewSpan();
            EndDrag();
            EndSizeRight();
            EndSizeLeft();
            UpdateToSource();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            ChangeSpanLength(e.GetPosition(canvas).X);
            DoDrag(e);

            SizeRight(e);
            SizeLeft(e);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BeginNewSpan(e);
        }

        #region 新建段

        private bool IsBeginNewSpan;
        private Span newSpan;
        private bool IsChangeSpanLength { get { return IsBeginNewSpan; } }

        /// <summary>
        /// 更改间隙长度
        /// </summary>
        private void ChangeSpanLength(double left)
        {
            if (!IsChangeSpanLength) return;

            if (newSpan == null)
            {
                Span span = NewSpan();
                span.SetValue(System.Windows.Controls.Canvas.LeftProperty, left);
                newSpan = span;
                canvas.Children.Add(newSpan);

            }

            var width = left - (double)newSpan.GetValue(System.Windows.Controls.Canvas.LeftProperty);
            if (width < 0) return;

            bool notInOther = CheckNotInOtherSpan(left, newSpan);
            if (notInOther)
                newSpan.Width = width;
        }

        /// <summary>
        /// 检查位置是否在其他border里面
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        private bool CheckNotInOtherSpan(double left, Span self)
        {
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                var border = (Span)canvas.Children[i];
                var isNewBoeder = border == self;
                if (isNewBoeder) continue;

                var borderLeft = (double)border.GetValue(System.Windows.Controls.Canvas.LeftProperty);
                var borderRight = borderLeft + border.Width;
                var f1 = left > borderLeft;
                var f2 = left < borderRight;

                if (f1 && f2) return false;

            }

            return true;
        }

        /// <summary>
        /// 开始新的
        /// *在有拖拽时才新建端，见ChangeSpanLength()
        /// </summary>
        private void BeginNewSpan(MouseButtonEventArgs e)
        {
            bool notInOther = CheckNotInOtherSpan(e.GetPosition(canvas).X, newSpan);
            if (notInOther)
                IsBeginNewSpan = true;
        }

        /// <summary>
        /// 结束新的
        /// </summary>
        private void EndNewSpan()
        {
            if (IsBeginNewSpan)
            {
                IsBeginNewSpan = false;
                newSpan = null;
            }
        }

        #endregion

        private void NewSpan_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BeginDrag(sender, e);
            BeginSelecting();
        }

        private void NewSpan_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EndDrag();
            EndSelecting(sender);
            UpdateToSource();
        }

        //private void NewSpan_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    EndDrag();
        //}

        //private void NewSpan_MouseMove(object sender, MouseEventArgs e)
        //{
        //    DoDrag(e);
        //}

        #region 拖拽段

        private Span dragSpan;
        private bool isBeginDragSpan;
        private double dragLeft;

        private void EndDrag()
        {
            if (!isBeginDragSpan) return;

            if (dragSpan != null)
                dragSpan.Style = GetNormalStyle();
            dragSpan = null;
            isBeginDragSpan = false;
        }

        private void DoDrag(MouseEventArgs e)
        {
            if (!isBeginDragSpan) return;

            var dragLeftNow = e.GetPosition(canvas).X;
            var drag = dragLeftNow - dragLeft;
            dragLeft = dragLeftNow;
            var left = (double)dragSpan.GetValue(System.Windows.Controls.Canvas.LeftProperty);
            var leftNow = left + drag;
            var rightNow = leftNow + dragSpan.Width;

            // 边界检查
            if (leftNow < 0) leftNow = 0;
            if (rightNow > canvas.ActualWidth) leftNow = canvas.ActualWidth - dragSpan.Width;

            //碰撞检测
            var checkLeftOk = CheckNotInOtherSpan(leftNow, dragSpan);
            if (!checkLeftOk) return;
            var checkRightOk = CheckNotInOtherSpan(rightNow, dragSpan);
            if (!checkRightOk) return;

            dragSpan.SetValue(System.Windows.Controls.Canvas.LeftProperty, leftNow);
        }

        private void BeginDrag(object sender, MouseButtonEventArgs e)
        {
            dragSpan = (Span)sender;
            dragLeft = e.GetPosition(canvas).X;
            dragSpan.Style = GetDragStyle();
            isBeginDragSpan = true;
        }

        #endregion

        private Style GetNormalStyle()
        {
            var resourceKey = "SpanNormalStyle";
            Style style = GetStyleByKey(resourceKey);
            return style;
        }

        private Style GetStyleByKey(string resourceKey)
        {
            return (System.Windows.Style)FindResource(resourceKey);
        }

        private Style GetSelectedStyle()
        {
            var resourceKey = "SpanSelectedStyle";
            Style style = GetStyleByKey(resourceKey);
            return style;
        }

        private Style GetDragStyle()
        {
            var resourceKey = "SpanDragStyle";
            Style style = GetStyleByKey(resourceKey);
            return style;
        }

        /// <summary>
        /// 创建新的span
        /// </summary>
        /// <returns></returns>
        private Span NewSpan()
        {
            var span = new Span();

            span.Style = GetNormalStyle();

            span.MouseLeftButtonDown += NewSpan_MouseLeftButtonDown;
            //newSpan.MouseMove += NewSpan_MouseMove;
            //newSpan.MouseLeave += NewSpan_MouseLeave;
            span.MouseLeftButtonUp += NewSpan_MouseLeftButtonUp;
            //newSpan.SetValue(System.Windows.Controls.Canvas.TopProperty, 1);
            span.Height = canvas.ActualHeight;
            span.Focusable = true;
            return span;
        }

        /// <summary>
        /// 删除选中的span
        /// </summary>
        private void DeleteSelectedSpan()
        {
            if (selectedSpan != null)
                canvas.Children.Remove(selectedSpan);
        }

        /// <summary>
        /// 设置选中的span
        /// </summary>
        private bool SetSelectedSpan()
        {
            var span = selectedSpan;
            var begin = span.BeginValue;
            var end = span.EndValue;
            var left = begin * Ratio;
            var right = end * Ratio;
            var newWidth = right - left;
            if (left < 0) return false;
            if (newWidth < 0) return false;

            bool notInOther = CheckNotInOtherSpan(left, span);
            if (!notInOther) return false;

            bool rightnotInOther = CheckNotInOtherSpan(right, span);
            if (!rightnotInOther) return false;

            span.Width = newWidth;
            span.SetValue(Canvas.LeftProperty, left);
            return true;
        }

        #region 选中段

        Span selectedSpan;
        bool isBeginSelectSpan;

        private void BeginSelecting()
        {
            // 重置上一个样式
            if (selectedSpan != null)
            {
                selectedSpan.Style = GetNormalStyle();
            }

            isBeginSelectSpan = true;
            selectedSpan = null;
        }

        private void EndSelecting(object sender)
        {
            if (isBeginSelectSpan)
            {
                selectedSpan = (Span)sender;
                selectedSpan.Style = GetSelectedStyle();
                isBeginSelectSpan = false;
            }
        }

        #endregion


        #region 更改段
        bool isSizeLeft;
        bool isSizeRight;
        double xPre;

        private void SizeRight(MouseEventArgs e)
        {
            if (!isSizeRight) return;
            var xNow = e.GetPosition(canvas).X;
            var xDiff = xNow - xPre;
            xPre = xNow;
            var width = selectedSpan.Width;
            var newWidth = width + xDiff;

            var x = xNow;
            var span = selectedSpan;

            if (newWidth < 0) return;

            bool notInOther = CheckNotInOtherSpan(x, span);
            if (notInOther)
                span.Width = newWidth;
        }

        private void EndSizeRight()
        {
            if (!isSizeRight) return;
            isSizeRight = false;
        }

        private void BeginSizeRight(MouseButtonEventArgs e)
        {
            e.Handled = true;
            xPre = e.GetPosition(canvas).X;
            isSizeRight = true;
        }

        private void BeginSizeLeft(MouseButtonEventArgs e)
        {
            e.Handled = true;
            xPre = e.GetPosition(canvas).X;
            isSizeLeft = true;
        }

        private void EndSizeLeft()
        {
            if (!isSizeLeft) return;
            isSizeLeft = false;
        }

        private void SizeLeft(MouseEventArgs e)
        {
            if (!isSizeLeft) return;
            var xNow = e.GetPosition(canvas).X;
            var xDiff = xNow - xPre;
            xPre = xNow;
            var width = selectedSpan.Width;
            var newWidth = width + (-xDiff);

            var x = xNow;
            var span = selectedSpan;

            if (newWidth < 0) return;

            bool notInOther = CheckNotInOtherSpan(x, span);
            if (notInOther)
                span.Width = newWidth;

            var left = (double)span.GetValue(Canvas.LeftProperty);
            var leftNow = left + xDiff;
            if (leftNow < 0) leftNow = 0;

            var checkLeftOk = CheckNotInOtherSpan(leftNow, span);
            if (!checkLeftOk) return;

            span.SetValue(Canvas.LeftProperty, leftNow);
        }

        #endregion

        #region 标尺部分


        #endregion

        private void leftEllipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BeginSizeLeft(e);
        }

        private void leftEllipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EndSizeLeft();
            UpdateToSource();
        }

        private void rightEllipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BeginSizeRight(e);
        }

        private void rightEllipse_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            EndSizeRight();
            UpdateToSource();
        }


        /// <summary>
        /// 总长度
        /// </summary>
        public double Lenght { get { return ActualWidth; } }

        private double ratio = 0;
        /// <summary>
        /// 控件长度和值的比率
        /// </summary>
        private double Ratio
        {
            get
            {
                if (ratio == 0)
                    ratio = Lenght / MaxValue;
                return ratio;
            }
            set
            {
                ratio = value;
            }
        }

        /// <summary>
        /// 值变化时执行
        /// </summary>
        private void ValuesChanged()
        {
            UpdateToUI();
        }

        /// <summary>
        /// 更新UI
        /// </summary>
        private void UpdateToUI()
        {
            if (flag == true) return;

            canvas.Children.Clear();
            if (MaxValue <= 0) return;
            if (Values == null) return;
            foreach (var item in Values)
            {
                var left = item[0];
                var width = item[1];

                left = left * Ratio;
                width = width * Ratio;

                var span = NewSpan();
                span.SetValue(Canvas.LeftProperty, left);
                span.Width = width;

                canvas.Children.Add(span);
            }
        }

        /// <summary>
        /// 更新值
        /// </summary>
        private void UpdateToSource()
        {
            var values = new List<double[]>();
            foreach (var item in canvas.Children)
            {
                var value = new double[2];
                values.Add(value);

                var span = (Span)item;
                var left = (double)span.GetValue(Canvas.LeftProperty);
                var width = span.ActualWidth;
                left = left / Ratio;
                width = width / Ratio;
                value[0] = left;
                value[1] = width;
                span.BeginValue = left;
                span.EndValue = left + width;
            }

            flag = true;
            Values = values;
            flag = false;
        }

        bool flag = false;

        /// <summary>
        /// 最大值变化时执行
        /// </summary>
        private void MaxValueChanged()
        {
            UpdateToUI();
        }


        private void UnitValueChanged()
        {
            UpdateMarkUI();
        }

        private void UnitLevelsChanged()
        {
            UpdateMarkUI();
        }

        /// <summary>
        /// 更新刻度
        /// </summary>
        private void UpdateMarkUI()
        {
            if (UnitValue == 0) return;
            if (UnitLevels == null) return;
            int levelsCount = UnitLevels.Count;

            var allMarkCount = MaxValue / UnitValue;
            var maxMarkHeigth = markCanvas.ActualHeight;

            for (int i = 0; i <= allMarkCount; i++)
            {
                var mark = new Mark();
                markCanvas.Children.Add(mark);
                int level = 1;
                for (; level <= levelsCount; level++)
                {
                    var x = UnitLevels[levelsCount - level];
                    if (i % x == 0)
                    {
                        break;
                    }
                }

                var value = i * UnitValue;
                var left = value * Ratio;
                var isFirst = i == 0;
                var isLast = i == allMarkCount;
                if (isFirst) left = 0;
                if (isLast) left = Lenght - 1;
                mark.Style = (System.Windows.Style)FindResource("markLevelStyle");
                if (level == 1)
                    mark.TitleVisibility = Visibility.Visible;
                mark.Height = maxMarkHeigth / level;
                mark.Value = value;
                mark.SetValue(Canvas.LeftProperty, left);
                mark.SetValue(Canvas.TopProperty, maxMarkHeigth - mark.Height);
            }

        }

        /// <summary>
        /// 值的列表，每项值为数组[开始，长度]
        /// </summary>
        public List<double[]> Values
        {
            get { return (List<double[]>)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValuesProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register("Values", typeof(List<double[]>), typeof(TimeSpanSet), new PropertyMetadata(null, new PropertyChangedCallback(ValuesPropertyChangedCallback)));


        public static void ValuesPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TimeSpanSet)d;
            control.ValuesChanged();
        }

        /// <summary>
        /// 总大小
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(TimeSpanSet), new PropertyMetadata(0.0, new PropertyChangedCallback(MaxValuePropertyChangedCallback)));

        public static void MaxValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TimeSpanSet)d;
            control.MaxValueChanged();
        }


        /// <summary>
        /// 标度单位大小
        /// </summary>
        public double UnitValue
        {
            get { return (double)GetValue(UnitValueProperty); }
            set { SetValue(UnitValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitValueProperty =
            DependencyProperty.Register("UnitValue", typeof(double), typeof(TimeSpanSet), new PropertyMetadata(0.0, new PropertyChangedCallback(UnitValuePropertyChangedCallback)));

        public static void UnitValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TimeSpanSet)d;
            control.UnitValueChanged();
        }


        /// <summary>
        /// 标度等级，
        /// eg：{1，2，3}，表示为，有3个等级的刻度，
        /// 第一个等级为每1个单位一个等级
        /// 第二个等级为每2个单位一个等级
        /// 第三个等级为每3个单位一个等级（一般情况是2的次幂，4）
        /// </summary>
        public List<int> UnitLevels
        {
            get { return (List<int>)GetValue(UnitLevelsProperty); }
            set { SetValue(UnitLevelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Levels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitLevelsProperty =
            DependencyProperty.Register("UnitLevels", typeof(List<int>), typeof(TimeSpanSet), new PropertyMetadata(null, new PropertyChangedCallback(UnitLevelsPropertyChangedCallback)));

        public static void UnitLevelsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TimeSpanSet)d;
            control.UnitLevelsChanged();
        }


        private void SetButton_Click(object sender, RoutedEventArgs e)
        {
            var ok = SetSelectedSpan();
            if (!ok) UpdateToSource();
        }

        private void DeleteBtnClick(object sender, RoutedEventArgs e)
        {
            DeleteSelectedSpan();
        }

        private void StackPanel_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }
    }
}
