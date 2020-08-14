using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class RSAExExample : InterFace.IProgramRun
    {
        private const string choice1 = "1";
        private const string choice2 = "2";
        private const string exit = "exit";
        public void Run()
        {
        GG:
            RSAExample rsaExample = new RSAExample();
            rsaExample.CreatKeyPairGuide();

            Console.WriteLine("输入exit退出...");
            Console.WriteLine("是否创建密码:是{0}, 否{1}", choice1, choice2);
            switch (Console.ReadLine())
            {
                case choice1: CreateKey(); break;
                case exit: return;
                default: break;
            }

            Console.WriteLine("请选择:加密{0},解密{1}", choice1, choice2);
            var choice = Console.ReadLine();

            switch (choice)
            {
                case choice1: Encrypt(); break;
                case choice2: Decrypt(); break;
                case exit: return;
                default: break;
            }
            goto GG;
        }

        private static void CreateKey()
        {
            Console.WriteLine("请选择公钥文件");
            var fileName = Console.ReadLine();
            if (!System.IO.File.Exists(fileName))
            {
                return;
            }

            using (var sr = System.IO.File.OpenText(fileName))
            {
                var publicKeyXml = sr.ReadToEnd();
                Security.RSAAES rSAAES = new Security.RSAAES();
                var keyBuffer = rSAAES.CreateKeyByRSAEncrypt(publicKeyXml);
                var keyFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.AES.key", Guid.NewGuid().ToString()));
                using (var fs = System.IO.File.Create(keyFileName))
                {
                    fs.Write(keyBuffer, 0, keyBuffer.Length);
                    fs.Flush();
                    Console.WriteLine("已创建密码文件:{0}", keyFileName);
                }
            }

        }

        private static void Encrypt()
        {
            Console.WriteLine("请选择加密文件");
            string fileName = Console.ReadLine();
            if (!System.IO.File.Exists(fileName)) return;

            Console.WriteLine("请选择密码文件");
            string keyFileName = Console.ReadLine();
            if (!System.IO.File.Exists(keyFileName)) return;

            Console.WriteLine("请选择私钥文件");
            string privateKeyFileName = Console.ReadLine();
            if (!System.IO.File.Exists(privateKeyFileName)) return;


            using (var keyFile = System.IO.File.Open(keyFileName, System.IO.FileMode.Open))
            {
                // 密码文件
                var keyData = new byte[keyFile.Length];
                keyFile.Read(keyData, 0, keyData.Length);
                using (System.IO.StreamReader sr = System.IO.File.OpenText(privateKeyFileName))
                {
                    // 私钥
                    var privateKeyXml = sr.ReadToEnd();
                    using (var fs = System.IO.File.Open(fileName, System.IO.FileMode.Open))
                    {
                        var fileData = new byte[fs.Length];
                        fs.Read(fileData, 0, fileData.Length);

                        Security.RSAAES rSAAES = new Security.RSAAES();
                        var data = rSAAES.Encrypt(fileData, keyData, privateKeyXml);
                        string aesFile = string.Format("{0}.aes", fileName);
                        using (var aesFileStream = System.IO.File.Create(aesFile))
                        {
                            aesFileStream.Write(data, 0, data.Length);
                            aesFileStream.Flush();
                            Console.WriteLine("加密文件:{0}", aesFile);
                        }
                    }
                }
            }


        }

        private static void Decrypt()
        {
            Console.WriteLine("请选择待解密文件");
            string fileName = Console.ReadLine();
            if (!System.IO.File.Exists(fileName)) return;

            Console.WriteLine("请选择密码文件");
            string keyFileName = Console.ReadLine();
            if (!System.IO.File.Exists(keyFileName)) return;

            Console.WriteLine("请选择私钥文件");
            string privateKeyFileName = Console.ReadLine();
            if (!System.IO.File.Exists(privateKeyFileName)) return;


            using (var keyFile = System.IO.File.Open(keyFileName, System.IO.FileMode.Open))
            {
                // 密码文件
                var keyData = new byte[keyFile.Length];
                keyFile.Read(keyData, 0, keyData.Length);
                using (System.IO.StreamReader sr = System.IO.File.OpenText(privateKeyFileName))
                {
                    // 私钥
                    var privateKeyXml = sr.ReadToEnd();
                    using (var fs = System.IO.File.Open(fileName, System.IO.FileMode.Open))
                    {
                        var fileData = new byte[fs.Length];
                        fs.Read(fileData, 0, fileData.Length);

                        Security.RSAAES rSAAES = new Security.RSAAES();
                        var data = rSAAES.Decrypt(fileData, keyData, privateKeyXml);
                        Console.WriteLine(System.Text.UTF8Encoding.UTF8.GetString(data));

                        Console.WriteLine("是否保存为解密文件: 是1,否2");
                        var choice = Console.ReadLine();
                        if (choice != "1") return;

                        string oriFile = string.Format("{0}.ori", fileName);
                        using (var oriFileStream = System.IO.File.Create(oriFile))
                        {
                            oriFileStream.Write(data, 0, data.Length);
                            oriFileStream.Flush();
                            Console.WriteLine("解密文件:{0}", oriFile);
                        }
                    }
                }
            }
        }
    }
}
