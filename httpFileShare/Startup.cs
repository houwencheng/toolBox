using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace httpFileShare
{
    class Startup
    {
        public void Configuration(Owin.IAppBuilder app)
        {
            app.UseNancy();
        }
    }
}
