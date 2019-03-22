using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class DBFactory
    {
        public DB GetNewDB(DBType dbType)
        {
            switch (dbType)
            {
                case DBType.MySql: return new MysqlDB();
                case DBType.SqlServer: return new SqlServerDB();
                default: return null;
            }
        }
    }
}
