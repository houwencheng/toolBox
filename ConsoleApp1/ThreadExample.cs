using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    /// <summary>
    /// 线程调用，行为测试
    /// </summary>
    public class ThreadExample : InterFace.IProgramRun
    {
        public void Run()
        {
            var parallelDemo = new ParallelDemo();
            parallelDemo.ParallelInvokeMethod();
            Console.ReadLine();
            return;
            //Console.WriteLine("a" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            //TestMethod();
            //Console.WriteLine("aa" + System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        private static async void TestMethod()
        {
            Console.WriteLine("TestMethod" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            await TestMethod1();
            Console.WriteLine("TestMethod" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            await TestMethod2();
            Console.WriteLine("TestMethod" + System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        private static Task TestMethod1()
        {
            return Task.Run(() =>
            {
                System.Threading.Thread.Sleep(5000);
            });
        }

        private static Task TestMethod2()
        {
            return Task.Run(() =>
            {
                System.Threading.Thread.Sleep(3000);
            });
        }


        public class ParallelDemo
        {
            private System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

            public void Run1()
            {
                //Thread.Sleep(2000);
                //Console.WriteLine("Task 1 is cost 2 sec");
                double sum = new Random().NextDouble();
                //double sum = 0;
                while (sum < 2e9)
                {
                    //var buffer = new byte[1024];
                    //var lenght = buffer.Length;
                    sum += 1;
                }

                Console.WriteLine(string.Format("[{1}]{0}", sum, System.Threading.Thread.CurrentThread.ManagedThreadId));
            }

            public double Run2()
            {
                Run1();
                return new Random().NextDouble();
            }

            public void ParallelInvokeMethod()
            {
                //stopWatch.Start();
                //Parallel.Invoke(Run1, Run1, Run1, Run1);
                //stopWatch.Stop();
                //Console.WriteLine("Parallel run " + stopWatch.ElapsedMilliseconds + " ms.");

                stopWatch.Restart();
                var sum = 0d;
                for (int i = 0; i < 4; i++)
                {
                    sum += Run2();
                }

                ////Run2();
                stopWatch.Stop();
                Console.WriteLine("Normal run " + stopWatch.ElapsedMilliseconds + " ms.");


                //Parallel.Invoke(Run1, Run1, Run1, Run1, Run1);

                //var threadCount = 5;
                //while (threadCount-- > 0)
                //{
                //    System.Threading.ThreadPool.QueueUserWorkItem((obj) =>
                //    {
                //        Run1();
                //    });
                //}
            }

        }
    }
}
