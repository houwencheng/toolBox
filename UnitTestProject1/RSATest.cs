using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class RSATest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string str = "hello world";
            var encoding = new System.Text.UTF8Encoding();
            var strBuffer= encoding.GetBytes(str);
            Security.RSA rsa = new Security.RSA();
            var keyPair = rsa.NewXmlKeyPair();

            var dataBuffer = rsa.Encrypt(strBuffer, keyPair[Security.KeyType.PublicKey]);
            var infoBuffer = rsa.Decrypt(dataBuffer, keyPair[Security.KeyType.PrivateKey]);


            var encryptStr = encoding.GetString(dataBuffer);
            var decryptStr = encoding.GetString(infoBuffer);
        }
    }
}
