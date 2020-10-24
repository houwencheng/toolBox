using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfControls
{
    /// <summary>
    /// 带提示的文本框
    /// </summary>
    public class NoticeTextBox : System.Windows.Controls.TextBox
    {
        bool isOnNoticeState = false;

        //static NoticeTextBox()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(NoticeTextBox), new FrameworkPropertyMetadata(typeof(NoticeTextBox)));
        //}

        public NoticeTextBox()
        {
            GotFocus += NoticeTextBox_GotFocus;
            LostFocus += NoticeTextBox_LostFocus;
            TextChanged += NoticeTextBox_TextChanged;
        }

        /// <summary>
        /// 提示文本
        /// </summary>
        public string Notice
        {
            get { return (string)GetValue(NoticeProperty); }
            set { SetValue(NoticeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Notice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoticeProperty =
            DependencyProperty.Register("Notice", typeof(string), typeof(NoticeTextBox), new PropertyMetadata(null, new PropertyChangedCallback(PropertyChangedCallback)));

        public static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var noticeTextBox = (NoticeTextBox)d;
            if (!string.IsNullOrEmpty(noticeTextBox.Value)) return;
            noticeTextBox.SetOnNoticeState();
        }


        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(NoticeTextBox), new PropertyMetadata(null, new PropertyChangedCallback(ValuePropertyChangedCallback)));

        public static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var noticeTextBox = (NoticeTextBox)d;
            noticeTextBox.SetValue();
        }

        /// <summary>
        /// 设置到提示状态
        /// </summary>
        private void SetOnNoticeState()
        {
            SetForegroundOpacity(0.3);

            TextChanged -= NoticeTextBox_TextChanged;
            Text = Notice;
            TextChanged += NoticeTextBox_TextChanged;
            isOnNoticeState = true;
        }

        private void SetForegroundOpacity(double opacity)
        {
            if (Foreground.IsFrozen)
            {
                var sd = Foreground as SolidColorBrush;
                Color c = sd.Color;
                SolidColorBrush newBrush = new SolidColorBrush(c);
                newBrush.Opacity = opacity;
                Foreground = newBrush;
            }
            else
            {
                Foreground.Opacity = opacity;
            }
        }

        /// <summary>
        /// 设置值
        /// </summary>
        private void SetValue()
        {
            Text = Value;
        }


        private void NoticeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Value = Text;
            SetOnNomalState();
        }

        /// <summary>
        /// 设置到正常状态
        /// </summary>
        private void SetOnNomalState()
        {
            SetForegroundOpacity(1);
            isOnNoticeState = false;
        }

        private void NoticeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
                SetOnNoticeState();
        }

        private void NoticeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isOnNoticeState)
                Text = null;
        }

    }
}
