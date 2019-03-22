using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class CpuTestExample
    {
        public void CpuTest1(string[] args)
        {
            int threadsCount = 1;
            if (args.Length > 0)
            {
                threadsCount = int.Parse(args[0]);
            }
            PerformanceTest.CPUTest cpuTest = new PerformanceTest.CPUTest();
            cpuTest.MultiTheadAddTest(threadsCount);
        }
    }
}
