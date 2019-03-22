using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class AsyncExample
    {
        public int Neijia(int number)
        {
            int sum = 0; 
            while (number > 0)
            {
                sum += number--;
                System.Threading.Thread.Sleep(100);
            }

            return sum;
        }

        public  int NeijiaAsyn(int number)
        {
            //await
            return 0;
        }
    }
}
