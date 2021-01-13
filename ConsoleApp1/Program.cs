using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        //[STAThread]
        [MTAThread]
        static void Main(string[] args)
        {

            UnitTestProject1.NTPClientTest nTPClientTest = new UnitTestProject1.NTPClientTest();
            nTPClientTest.TestMethod1();
            Console.ReadLine();
            return;
            //InterFace.IProgramRun programRun = new ThreadExample();
            ////programRun = new MD5Example();
            ////programRun = new CpuTestExample();
            //programRun.Run();

            //WpfApp wpfApp = new WpfApp();
            //wpfApp.Run();
            var thread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
            {
                //System.Windows.Window window = new System.Windows.Window();
                //window.Content = new WpfControls.PlayTimeLine();
                //window.Closed += Window_Closed;
                //window.ShowDialog();

                WpfApp wpfApp = new WpfApp();
                wpfApp.Run();
            }));

            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();

            Console.Read();
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
