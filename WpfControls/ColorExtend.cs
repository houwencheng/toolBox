using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControls
{
    /// <summary>
    /// 颜色扩展
    /// </summary>
    public static class ColorExtend
    {
        public static System.Windows.Media.Brush ToWindowsBrush(this Model.Color color)
        {
            var color1 = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color.HexRGB);
            System.Windows.Media.Brush brush = new System.Windows.Media.SolidColorBrush(color1);
            return brush;
        }
    }
}
