using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    /// <summary>
    /// 元组测试
    /// </summary>
    public class TuplesTest : InterFace.IProgramRun
    {

        private static (string, int, double, string) QueryCityData(string name)
        {
            if (name == "New York City")
                return (name, 8175133, 468.48, "w ");

            return ("", 0, 0, "334");
        }

        public void Run()
        {
            var result = QueryCityData("New York City");

            var city = result.Item1;
            var pop = result.Item2;
            var size = result.Item3;
            var nothing = result.Item4;
            Console.WriteLine(city);
            Console.WriteLine(pop);
            Console.WriteLine(size);
        }
    }
}
