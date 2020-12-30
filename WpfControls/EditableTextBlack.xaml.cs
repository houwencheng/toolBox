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
    /// EditableTextBlack.xaml 的交互逻辑
    /// </summary>
    public partial class EditableTextBlack : UserControl
    {
        public EditableTextBlack()
        {
            InitializeComponent();
            textBlock.Focusable = true;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBlack), new PropertyMetadata(null, new PropertyChangedCallback(TextPropertyChangedCallback)));

        public static void TextPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (EditableTextBlack)d;
            var text = (string)e.NewValue;
            control.tb.Text = text;
            control.textBlock.Text = text;
        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenEditor();
        }


        public void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                OpenEditor();
            }
            else if (e.Key == Key.Enter)
            {
                CloseEditor();
            }

        }

        private void OpenEditor()
        {
            tb.Visibility = Visibility.Visible;
            textBlock.Visibility = Visibility.Hidden;
            tb.Text = Text;
            tb.SelectAll();
            tb.LostFocus -= Tb_LostFocus;
            tb.LostFocus += Tb_LostFocus;
            EditOpend = true;
        }

        private void Tb_LostFocus(object sender, RoutedEventArgs e)
        {
            CloseEditor();
        }

        private void CloseEditor()
        {
            Text = tb.Text;
            tb.Visibility = Visibility.Hidden;
            textBlock.Visibility = Visibility.Visible;
            //BindingExpression binding = this.GetBindingExpression(TextProperty);
            //binding.UpdateSource();
            EditOpend = false;
        }



        /// <summary>
        /// 打开编辑状态
        /// </summary>
        public bool EditOpend
        {
            get { return (bool)GetValue(EditOpendProperty); }
            set { SetValue(EditOpendProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditOpend.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditOpendProperty =
            DependencyProperty.Register("EditOpend", typeof(bool), typeof(EditableTextBlack), new PropertyMetadata(false));


    }
}
