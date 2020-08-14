using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    /// <summary>
    /// 外部排序
    /// </summary>
    public class OutShort : InterFace.IProgramRun
    {
        public void Run()
        {
            string dataFileName = "data";
            double dataCount = 1e1;
            uint mmSize = 4;
            //byte[] mm = new byte[mmSize * 8];
            double[] mm = new double[mmSize];

            BuildData(dataFileName, dataCount);

            SplitFile(dataFileName, mmSize);

            OrderSplitFile(dataFileName, mm);
        }

        private static void OrderSplitFile(string dataFileName, double[] mm)
        {
            var mmCount = mm.Count();
            System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(dir);
            var files = directoryInfo.GetFiles();
            foreach (var item in files)
            {
                if (item.Name.EndsWith("txt")) continue;
                var fileName = item.FullName;
                int i = 0;
                using (var fs = System.IO.File.OpenRead(fileName))
                {
                    while (true)
                    {
                        var buffer = new byte[8];
                        var readCount = fs.Read(buffer, 0, buffer.Length);
                        if (readCount == 0) break;

                        var d = BitConverter.ToDouble(buffer, 0);
                        mm[i] = d;
                        for (int j = 0; j < i; j++)
                        {
                            if (mm[i] < mm[j])
                            {
                                mm[mmCount - 1] = mm[j];
                                mm[j] = mm[i];
                                mm[i] = mm[mmCount - 1];
                                continue;
                            }
                        }

                        i++;
                    }
                }

                using (var fs = System.IO.File.OpenWrite(fileName))
                {
                    for (int j = 0; j < i; j++)
                    {
                        var buffer = BitConverter.GetBytes(mm[j]);
                        fs.Write(buffer, 0, buffer.Length);
                    }

                    fs.Flush();
                }

                BuildText(fileName);
            }
        }

        static string dir = "part";
        private static void SplitFile(string dataFileName, uint mmSize)
        {
            using (var fs = System.IO.File.OpenRead(dataFileName))
            {
                var exist = System.IO.Directory.Exists(dir);
                if (!exist)
                {
                    System.IO.Directory.CreateDirectory(dir);
                }


                int i = 0;
                while (true)
                {
                    var partSize = (mmSize - 1) * 8;
                    var buffer = new byte[partSize];
                    var readCount = fs.Read(buffer, 0, buffer.Length);
                    if (readCount == 0) break;
                    var fileName = string.Format("{1}/part{0}", ++i, dir);
                    using (var partFile = System.IO.File.Create(fileName))
                    {
                        partFile.Write(buffer, 0, readCount);
                        partFile.Flush();
                    }

                    //BuildText(fileName);
                }
            }
        }

        private static void BuildData(string dataFileName, double count)
        {
            using (var fs = System.IO.File.Create(dataFileName))
            {
                Random rd = new Random();
                for (int i = 0; i < count; i++)
                {
                    var d = rd.NextDouble();
                    var buffer = BitConverter.GetBytes(d);
                    fs.Write(buffer, 0, buffer.Length);

                }
                fs.Flush();
            }

            BuildText(dataFileName);
        }


        private static void BuildText(string dataFileName)
        {
            using (var fsTxt = System.IO.File.Create(string.Format("{0}.txt", dataFileName)))
            {
                using (var fs = System.IO.File.OpenRead(dataFileName))
                {
                    while (true)
                    {
                        var buffer = new byte[8];
                        var readCount = fs.Read(buffer, 0, buffer.Length);
                        if (readCount == 0) break;

                        var d = BitConverter.ToDouble(buffer, 0);
                        var bufferTxt = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}\t", d));
                        fsTxt.Write(bufferTxt, 0, bufferTxt.Length);
                    }

                }

                fsTxt.Flush();
            }
        }


    }
}
