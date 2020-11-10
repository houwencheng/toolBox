using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControls.Model
{
    /// <summary>
    /// 闭合框
    /// </summary>
    public class Path
    {
        public List<Point> Points { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
    }
}
