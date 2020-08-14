using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class MD5Example : InterFace.IProgramRun
    {
        public void Run()
        {
            string exitCode = "q";
            Console.WriteLine("输入{0}退出", exitCode);
            while (true)
            {
                Console.WriteLine("请选择要计算MD5的文件:");
                var fileName = Console.ReadLine();
                if (fileName == exitCode) Environment.Exit(0);
                if (!System.IO.File.Exists(fileName))
                    continue;

                using (var fs = System.IO.File.OpenRead(fileName))
                {
                    Security.MD5 md5 = new Security.MD5();
                    var md5String = md5.Compute(fs);
                    Console.WriteLine(md5String);
                }
            }
        }
    }
}
