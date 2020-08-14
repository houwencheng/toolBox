using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class CpuTestExample : InterFace.IProgramRun
    {
        public void Run()
        {
            int threadsCount = 1;

            PerformanceTest.CPUTest cpuTest = new PerformanceTest.CPUTest();
            cpuTest.MultiTheadAddTest(threadsCount);
        }
    }
}
