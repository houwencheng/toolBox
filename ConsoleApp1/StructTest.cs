using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class StructTest
    {
        //public StructA StructA { get; set; }
        public StructA StructA = new StructA();

        public StructTest()
        {
            StructA.i = 2;
            var buffer = System.Text.Encoding.UTF8.GetBytes("hello");
            StructA.o = new System.IO.MemoryStream(buffer);
        }

        public void InitStructA()
        {
            StructA = new StructA();
        }

    }

    public struct StructA
    {
        public int i;
        public string s;
        public object o;

        //public StructA()
        //{
        //    //i = 1;
        //    //s = "1";
        //    //o = new object();
        //}
    }
}
