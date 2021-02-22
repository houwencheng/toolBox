using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    delegate int SumDelegate(int x, int y);

    public static class Logger
    {
        public static Action<string> WriteMessage;

        public static int LogLevel { get; set; } = 1;

        public static void LogMessage(int s, string component, string msg)
        {
            if (s < LogLevel)
                return;

            var outputMsg = $"{DateTime.Now}\t{s}\t{component}\t{msg}";
            WriteMessage(outputMsg);
        }
    }

    class Program
    {
        private static int Sum(int x, int y)
        {
            return x + y;
        }

        //[STAThread]
        [MTAThread]
        static void Main(string[] args)
        {
            //var d1 = new { X = 1, Y = 2 };
            
            //SumDelegate sumDelegate = new SumDelegate(Sum);
            //List<int> list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            //var jishu = GetJishu(list);
            //foreach (var item in jishu)
            //{
            //    Console.WriteLine(item);
            //}

            //Logger.WriteMessage += LogMessage;

            //UnitTestProject1.NTPClientTest nTPClientTest = new UnitTestProject1.NTPClientTest();
            //nTPClientTest.TestMethod1();
            //Console.ReadLine();
            //return;
            InterFace.IProgramRun programRun = new TuplesTest();
            //programRun = new MD5Example();
            //programRun = new CpuTestExample();
            //programRun = new FileRename();
            programRun = new ThreadExample();
            programRun.Run();
            return;

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

        private static void LogMessage(string msg)
        {
            try
            {
                //using (var log = File.AppendText(logPath))
                //{
                //    log.WriteLine(msg);
                //    log.Flush();
                //}
            }
            catch (Exception)
            {
                // Hmm. We caught an exception while
                // logging. We can't really log the
                // problem (since it's the log that's failing).
                // So, while normally, catching an exception
                // and doing nothing isn't wise, it's really the
                // only reasonable option here.
            }
        }

        private static IEnumerable<int> GetJishu(List<int> list)
        {
            foreach (var item in list)
            {
                if (item % 2 == 1)
                    yield return item;
            }
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
