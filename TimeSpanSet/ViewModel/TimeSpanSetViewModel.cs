using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSpanSet.ViewModel
{
    public class TimeSpanSetViewModel
    {

        public List<double[]> Values { get; set; }

        public double MaxValue { get; set; }

        /// <summary>
        /// 标度单位
        /// </summary>
        public double UnitValue { get; set; }
        /// <summary>
        /// 标度等级
        /// </summary>
        public List<int> UnitLevels { get; set; }

        public TimeSpanSetViewModel()
        { 
            DeleteSpanCommand = new DeleteSpanCommand();
        }

        public System.Windows.Input.ICommand SetSpanCommand { get; set; }
        public System.Windows.Input.ICommand DeleteSpanCommand { get; set; }

    }


    public class DeleteSpanCommand : System.Windows.Input.ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var vm = (TimeSpanSetViewModel)parameter;
        }
    }
}
