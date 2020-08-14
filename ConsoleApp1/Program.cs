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
        //[STAThread]
        [MTAThread]
        static void Main(string[] args)
        {
            InterFace.IProgramRun programRun = new ThreadExample();
            programRun = new MD5Example();
            Run(programRun);
        }

        static void Run<T>(T programRun) where T : InterFace.IProgramRun
        {
            programRun.Run();
        }
    }
}
