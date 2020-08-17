using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfControls
{
    /// <summary>
    /// 图片按钮
    /// </summary>
    public class ImageBtn : Button
    {
        static ImageBtn()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageBtn), new FrameworkPropertyMetadata(typeof(ImageBtn)));
        }

        public ImageBtn()
        {
            MouseMove += ImageBtn_MouseMove;
        }

        private void ImageBtn_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public string NormalImage
        {
            get { return (string)GetValue(NormalImageProperty); }
            set { SetValue(NormalImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NomalImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NormalImageProperty =
            DependencyProperty.Register("NormalImage", typeof(string), typeof(ImageBtn), new PropertyMetadata(null, new PropertyChangedCallback(PropertyChangedCallback)));

        public static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }



        public string FocusImage
        {
            get { return (string)GetValue(FocusImageProperty); }
            set { SetValue(FocusImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FocusImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FocusImageProperty =
            DependencyProperty.Register("FocusImage", typeof(string), typeof(ImageBtn), new PropertyMetadata(null));



        public string PressImage
        {
            get { return (string)GetValue(PressImageProperty); }
            set { SetValue(PressImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PressImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PressImageProperty =
            DependencyProperty.Register("PressImage", typeof(string), typeof(ImageBtn), new PropertyMetadata(null));


    }
}
