using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace httpFileShare
{
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                Console.WriteLine("file share floder:");
                var shareFloder = Console.ReadLine();
                var floderNotOk = !System.IO.Directory.Exists(shareFloder);
                if (floderNotOk)
                    continue;
                NancyModule.FileModule.ShareFloder = shareFloder;
                break;
            }


            var url = "http://+:80";

            using (Microsoft.Owin.Hosting.WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Running on {0}", url);
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }
}
