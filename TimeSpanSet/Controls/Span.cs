using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimeSpanSet.Controls
{
    /// <summary>
    /// 长度条
    /// </summary>
    public class Span :System.Windows.Controls.Control
    {

        public double BeginValue
        {
            get { return (double)GetValue(BeginValueProperty); }
            set{ SetValue(BeginValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BeginValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeginValueProperty =
            DependencyProperty.Register("BeginValue", typeof(double), typeof(Span), new PropertyMetadata(0.0));

        public double EndValue
        {
            get { return (double)GetValue(EndValueProperty); }
            set { SetValue(EndValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(Span), new PropertyMetadata(0.0));

    }
}
