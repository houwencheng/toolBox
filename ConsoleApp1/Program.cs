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

        private static async Task GetPageSizeAsync(string url)
        {
            var client = new HttpClient();
            var uri = new Uri(Uri.EscapeUriString(url));
            byte[] urlContents = await client.GetByteArrayAsync(uri);
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine($"{url}: {urlContents.Length / 2:N0} characters");
        }

        [STAThread]
        static void Main(string[] args)
        {
            //new RSAExample().Run();
            new RSAExExample().Run();
            return;

            GetPageSizeAsync("http://www.baidu.com").Wait();

            DB.DBFactory dbFactory = new DB.DBFactory();
            var db = dbFactory.GetNewDB(DB.DBType.MySql);
            Console.WriteLine(db.Excute());

            db = dbFactory.GetNewDB(DB.DBType.SqlServer);
            Console.WriteLine(db.Excute());

            //DB.DB db = new DB.DB();

            DockFormFramework.Program.Run();
            //new SpeakerTestExample().SpeakHelloWord(20);
            //new CpuTestExample().CpuTest1(args);
            var mutiThreadTest = new MutiThreadTest();
            //mutiThreadTest.Run(10, 2, ()=> {
            //    mutiThreadTest.PrintResult();
            //});
            mutiThreadTest.RunAsync(int.MaxValue);
            Console.WriteLine("1:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);

            Console.ReadLine();
        }



    }
}
