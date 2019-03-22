using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class MysqlDB : DB
    {
        public override object Excute(params object[] param)
        {
            //return base.Excute(param);
            return "mysql excute";
        }
    }
}
