using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class DB
    {
        internal DB() { }
        public string ConnectionString { get; private set; }
        public virtual void Open(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public virtual void Close()
        { }

        public virtual object Excute(params object[] param)
        {
            throw new NotImplementedException();
        }
    }
}
