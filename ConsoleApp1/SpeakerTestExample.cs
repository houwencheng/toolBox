using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class SpeakerTestExample
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="times">次数</param>
        public void SpeakHelloWord(int times)
        {
            //int times = 20;
            while (times > 0)
            {
                Device.Speaker speaker = new Device.Speaker();
                var str = string.Format("helloword 欢迎 {0}", times);
                Console.WriteLine(str);
                speaker.SpeakAsync(str);
                times--;
                //break;
            }

            Console.WriteLine("press any key to exit.");
            Console.ReadLine();
        }
    }
}
