using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimeSpanSet.Controls
{
    /// <summary>
    /// 标度
    /// </summary>
    public class Mark : System.Windows.Controls.Control
    {
        public System.Windows.Visibility TitleVisibility
        {
            get { return (System.Windows.Visibility)GetValue(TitleVisibilityProperty); }
            set { SetValue(TitleVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Titel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleVisibilityProperty =
            DependencyProperty.Register("TitleVisibility", typeof(System.Windows.Visibility), typeof(Mark), new PropertyMetadata(System.Windows.Visibility.Hidden));


        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(Mark), new PropertyMetadata(0.0));

    }

}
