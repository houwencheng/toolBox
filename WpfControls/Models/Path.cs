using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControls.Models
{
    /// <summary>
    /// 闭合框
    /// </summary>
    public class Path
    {
        public Level Level { get; set; }
        public List<System.Windows.Point> Points { get; set; }
    }
}
