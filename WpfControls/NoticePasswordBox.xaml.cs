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
    /// 带提示的密码框
    /// </summary>
    public partial class NoticePasswordBox : UserControl
    {
        public NoticePasswordBox()
        {
            InitializeComponent();
        }


        public string Notice
        {
            get { return (string)GetValue(NoticeProperty); }
            set { SetValue(NoticeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Notice.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoticeProperty =
            DependencyProperty.Register("Notice", typeof(string), typeof(NoticePasswordBox), new PropertyMetadata(null, new PropertyChangedCallback(PropertyChangedCallback)));

        public static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NoticePasswordBox)d;
            if (control.CanShowNotice)
                control.SetOnNoticeState();
        }

        private bool CanShowNotice
        {
            get
            {
                return string.IsNullOrEmpty(Password);
            }
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(NoticePasswordBox), new PropertyMetadata(null, new PropertyChangedCallback(PasswordPropertyChangedCallback)));


        public static void PasswordPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NoticePasswordBox)d;
            control.SetOnNomalState();
        }


        /// <summary>
        /// 设置到提示状态
        /// </summary>
        private void SetOnNoticeState()
        {
            this.passwordBox.Visibility = System.Windows.Visibility.Hidden;
            this.textBox.Visibility = System.Windows.Visibility.Visible;
            var color = (Color)ColorConverter.ConvertFromString("#ADB0BB");
            this.textBox.Foreground = new SolidColorBrush(color);
            color = (Color)ColorConverter.ConvertFromString("#AEB4CF");
            this.textBox.BorderBrush = new SolidColorBrush(color);
            this.textBox.Text = this.Notice;
        }

        /// <summary>
        /// 设置到正常状态
        /// </summary>
        private void SetOnNomalState()
        {
            var color = (Color)ColorConverter.ConvertFromString("#4F6DE5");
            this.passwordBox.BorderBrush = new SolidColorBrush(color);
            this.passwordBox.Visibility = System.Windows.Visibility.Visible;
            this.textBox.Visibility = System.Windows.Visibility.Hidden;

            if (this.passwordBox.Password == Password) return;
            this.passwordBox.PasswordChanged -= passwordBox_PasswordChanged;
            this.passwordBox.Password = Password;
            this.passwordBox.PasswordChanged += passwordBox_PasswordChanged;
        }

        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.Password = passwordBox.Password;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SetOnNomalState();
            this.passwordBox.Focus();
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CanShowNotice)
            {
                SetOnNoticeState();
            }

            IsFocused2 = false;
        }

        private void passwordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            IsFocused2 = true;
        }



        public bool IsFocused2
        {
            get { return (bool)GetValue(IsFocused2Property); }
            set { SetValue(IsFocused2Property, value); }
        }

        // Using a DependencyProperty as the backing store for IsFocused2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFocused2Property =
            DependencyProperty.Register("IsFocused2", typeof(bool), typeof(NoticePasswordBox), new PropertyMetadata(false));


    }
}
