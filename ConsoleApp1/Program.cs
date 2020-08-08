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
            var html = System.Text.ASCIIEncoding.UTF8.GetString(urlContents);
            Console.WriteLine(html);
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine($"{url}: {urlContents.Length / 2:N0} characters");
        }

        System.Threading.AutoResetEvent autoResetEvent = new System.Threading.AutoResetEvent(false);

        //[STAThread]
        [MTAThread]
        static void Main(string[] args)
        {
            StructTest structTest = new StructTest();
            //structTest.InitStructA();

            var A = structTest.StructA;
            A.i = 123;
            var buffer = System.Text.Encoding.UTF8.GetBytes("hello2");
            A.o = new System.IO.MemoryStream(buffer);
            string s = GetString(A);
            string s2 = GetString(structTest.StructA);

            Console.WriteLine(s);
            return;
            OutShort.Main_(args);
            return;

            MD5Example mD5Example = new MD5Example();
            mD5Example.Run();
            //new PerformanceTest.CPUTest().MultiTheadAddTest(1);

            var program = new Program();
            //double result = 0;
            //Task.Run(() =>
            //{
            //    var t = program.Sum(5);
            //    result = t.Result;
            //    program.autoResetEvent.Set();
            //});

            Console.WriteLine("Please wait");
            //program.autoResetEvent.WaitOne();
            var t = program.Sum(5);
            var flag = t.Wait(1000);
            var result = t.Result;

            //var result = program.Sum(5).Result;
            Console.WriteLine("Result:{0}", result);
            Console.WriteLine("Please press any key to exit");
            Console.ReadKey();
            return;



            //new RSAExample().Run();
            //new RSAExExample().Run();
            //return;
            new MD5Example().Run();
            Console.ReadKey();
            //GetPageSizeAsync("http://www.baidu.com").Wait();
            return;

            DB.DBFactory dbFactory = new DB.DBFactory();
            var db = dbFactory.GetNewDB(DB.DBType.MySql);
            Console.WriteLine(db.Excute());

            db = dbFactory.GetNewDB(DB.DBType.SqlServer);
            Console.WriteLine(db.Excute());

            //DB.DB db = new DB.DB();

<<<<<<< HEAD
=======
            //DockFormFramework.Program.Run();
>>>>>>> 1b4b4de05b0726f6ad527c649906e0fd1e1fe3b7
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

        private static string GetString(StructA A)
        {
            var stream = (System.IO.MemoryStream)A.o;
            var length = stream.Length;
            var bufer = new byte[length];
            stream.Read(bufer, 0, (int)length);
            var s = System.Text.Encoding.UTF8.GetString(bufer);
            return s;
        }

        public async Task<double> Sum(int times)
        {
            return await Task.Run<double>(() =>
            {
                double sum = 0;
                while (times-- > 0)
                {
                    System.Threading.Thread.Sleep(1000);
                    sum += times;
                }

                return sum;
            });
        }
    }
}
