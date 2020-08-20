using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfControls.Models
{
    /// <summary>
    /// 等级
    /// </summary>
    public class Level
    {
        public int Value { get; set; }
        public string Name { get; set; }
        //[Newtonsoft.Json.JsonIgnore]
        public Brush Brush { get; set; }
    }
}
