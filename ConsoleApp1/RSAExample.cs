using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class RSAExample
    {
        const string choice1 = "1";
        const string choice2 = "2";

        const string exit = "exit";

        public void Run()
        {
        GC:
            CreatKeyPairGuide();

            Console.WriteLine("请选择:加密{0},解密{1}", choice1, choice2);
            var choice = Console.ReadLine();

            switch (choice)
            {
                case choice1: Encrypt(); break;
                case choice2: Decrypt(); break;
                case exit: return;
                default: break;
            }

            goto GC;
        }

        public void CreatKeyPairGuide()
        {
            Console.WriteLine("输入{0}退出...", exit);
            Console.WriteLine("是否创建密钥对:是{0},否{1}", choice1, choice2);
            var choice = Console.ReadLine();
            switch (choice)
            {
                case choice1: CreateKeyPair(); break;
                case choice2:; break;
                case exit: Environment.Exit(0); break;
                default: break;
            }
        }

        private static void CreateKeyPair()
        {
            Security.RSA rsa = new Security.RSA();
            var keyPair = rsa.NewXmlKeyPair();

            var publicKeyXml = keyPair[Security.KeyType.PublicKey];
            var privateKeyXml = keyPair[Security.KeyType.PrivateKey];
            var guid = Guid.NewGuid().ToString();
            var publicKeyFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.public", guid));
            var privateKeyFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("{0}.private", guid));
            using (System.IO.StreamWriter sw = System.IO.File.CreateText(publicKeyFileName))
            {
                sw.Write(publicKeyXml);
                sw.Flush();
                Console.WriteLine("公钥文件:{0}", publicKeyFileName);
            }

            using (System.IO.StreamWriter sw = System.IO.File.CreateText(privateKeyFileName))
            {
                sw.Write(privateKeyXml);
                sw.Flush();
                Console.WriteLine("私钥文件:{0}", privateKeyFileName);
            }
        }

        private static void Encrypt()
        {
            Console.WriteLine("请选择待加密文件");
            string fileName = Console.ReadLine();
            if (!System.IO.File.Exists(fileName)) return;

            Console.WriteLine("请选择公钥文件");
            string keyFileName = Console.ReadLine();
            if (!System.IO.File.Exists(keyFileName)) return;


            using (var fs = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                var fileData = new byte[fs.Length];
                fs.Read(fileData, 0, fileData.Length);
                using (System.IO.StreamReader sr = System.IO.File.OpenText(keyFileName))
                {
                    var keyXml = sr.ReadToEnd();
                    Security.RSA rsa = new Security.RSA();
                    var data = rsa.Encrypt(fileData, keyXml);

                    string newFileName = string.Format("{0}.rsa", fileName);
                    using (var newfs = System.IO.File.Create(newFileName))
                    {
                        newfs.Write(data, 0, data.Length);
                        newfs.Flush();
                        Console.WriteLine("加密文件:{0}", newFileName);
                    }
                }
            }


        }

        private static void Decrypt()
        {
            Console.WriteLine("请选择待解密文件");
            string fileName = Console.ReadLine();
            if (!System.IO.File.Exists(fileName)) return;

            Console.WriteLine("请选择私钥文件");
            string keyFileName = Console.ReadLine();
            if (!System.IO.File.Exists(keyFileName)) return;

            using (var fs = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                var fileData = new byte[fs.Length];
                fs.Read(fileData, 0, fileData.Length);
                using (System.IO.StreamReader sr = System.IO.File.OpenText(keyFileName))
                {
                    var keyXml = sr.ReadToEnd();
                    Security.RSA rsa = new Security.RSA();
                    var data = rsa.Decrypt(fileData, keyXml);
                    Console.WriteLine(System.Text.UTF8Encoding.UTF8.GetString(data));

                    Console.WriteLine("是否保存为解密文件: 是1,否2");
                    var choice = Console.ReadLine();
                    if (choice != "1") return;
                    string newFileName = string.Format("{0}.ori", fileName);
                    using (var newfs = System.IO.File.Create(newFileName))
                    {
                        newfs.Write(data, 0, data.Length);
                        newfs.Flush();
                        Console.WriteLine("解密文件:{0}", newFileName);
                    }
                }
            }
        }
    }
}
