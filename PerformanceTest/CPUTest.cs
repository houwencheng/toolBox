using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTest
{
    public class CPUTest
    {
        /// <summary>
        /// cpu性能测试，多线程累加运算
        /// </summary>
        /// <param name="threadsCount">线程数</param>
        public void MultiTheadAddTest(int threadsCount)
        {
            OnOff onOff = new OnOff();
            onOff.StartTime = DateTime.Now;

            Console.WriteLine("threadCount:{0}", threadsCount);
            for (int i = 0; i < threadsCount; i++)
            {
                AsynAdd(onOff);
            }

            onOff.IsOff = Console.ReadLine().Equals("q");
            Console.ReadLine();
        }

        /// <summary>
        /// 异步累加
        /// </summary>
        /// <param name="onOff"></param>
        private void AsynAdd(OnOff onOff)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(obj =>
            {
                var onOff_ = (OnOff)obj;
                DateTime timeStart = onOff_.StartTime;
                int i = 0;
                while (!onOff_.IsOff)
                {
                    i++;
                    if (i < 0) break;
                }
                DateTime timeStop = DateTime.Now;

                Console.WriteLine("{0}\t{1}", System.Threading.Thread.CurrentThread.ManagedThreadId, timeStop - timeStart);
            }, onOff);
        }
    }
}
