using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class MutiThreadObject
    {
        public int Count { get; set; }
    }

    public delegate void CallBack();
    public class MutiThreadTest
    {
        public MutiThreadObject MutiThreadObject = new MutiThreadObject();
        public void Run(int times, int threadsCount, CallBack callBack)
        {
            for (int i = 0; i < threadsCount; i++)
            {
                //System.Threading.ThreadPool.QueueUserWorkItem(obj =>
                //{
                //    int timesLocal = times;
                //    var data = (MutiThreadObject)obj;
                //    while (timesLocal-- > 0)
                //    {
                //        ++data.Count;
                //    }
                //    Console.WriteLine("thread {0} complate", System.Threading.Thread.CurrentThread.Name);
                //}, MutiThreadObject);
                System.Threading.ThreadPool.QueueUserWorkItem(obj =>
                {
                    Count(times, callBack);
                });
            }
        }

        private void Count(int times, CallBack callBack)
        {
            var managedThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            int timesLocal = times;
            while (timesLocal-- > 0)
            {
                //lock (MutiThreadObject)
                //{
                //Console.WriteLine("thread {0},count {1}", managedThreadId, ++MutiThreadObject.Count);
                MutiThreadObject.Count++;
                //}
            }

            callBack?.Invoke();

        }

        public async void RunAsync(int times)
        {
            string s = "SSSSSSSSSSSSS";
            var task = new Task<int>((str) =>
            {
                //Count(times, null);
                Console.WriteLine("2:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
                System.Threading.Thread.Sleep(5000);
                System.Threading.ThreadPool.QueueUserWorkItem((obj) =>
                {
                    Console.WriteLine("2:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
                });
                str = "uuuuuuuuuuuuu";
                return MutiThreadObject.Count;
            }, s);

            task.Start();
            Console.WriteLine("3:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            int result = await task;
            Console.WriteLine("3:{0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine(s);
        }

        public void PrintResult()
        {
            Console.WriteLine("{0}", MutiThreadObject.Count);
        }
    }
}
