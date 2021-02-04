using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class FileRename : InterFace.IProgramRun
    {
        public void Run()
        {
            begin:
            Console.WriteLine("\n\n\n待排序文件夹路径:");
            var dir = Console.ReadLine();
            var existDir = System.IO.Directory.Exists(dir);
            if (!existDir)
            {
                Console.WriteLine(string.Format("路径不存在,{0}", dir));
                goto begin;
            }

            Console.WriteLine("前缀:");
            var prefix = Console.ReadLine();
            Console.WriteLine("后缀:");
            var suffix = Console.ReadLine();

            var files = System.IO.Directory.GetFiles(dir);
            int orderNumber = 1;
            var map = files.Select(file => new FileRenameModel
            {
                OriName = file,
                GuidName = string.Format("{0}/{1}", dir, Guid.NewGuid()),
                NewName = string.Format("{0}/{1}{2}{3}", dir, prefix, orderNumber++, suffix)
            }).ToList();

            //改成guid名
            foreach (var item in map)
            {
                var fileInfo = new System.IO.FileInfo(item.OriName);
                fileInfo.MoveTo(item.GuidName);
            }


            //改成序号名
            foreach (var item in map)
            {
                var fileInfo = new System.IO.FileInfo(item.GuidName);
                fileInfo.MoveTo(item.NewName);
            }

            goto begin;
        }

        /// <summary>
        /// 重命名模型
        /// </summary>
        internal class FileRenameModel
        {
            /// <summary>
            /// 原始名
            /// </summary>
            public string OriName { get; set; }

            /// <summary>
            /// 过渡guid文件名
            /// </summary>
            public string GuidName { get; set; }

            /// <summary>
            /// 新名称
            /// </summary>
            public string NewName { get; set; }
        }
    }
}
