using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class RSA
    {
        /// <summary>
        /// 生成密钥对,密钥为Xml格式
        /// </summary>
        /// <returns></returns>
        public Dictionary<KeyType, string> NewXmlKeyPair()
        {
            Dictionary<KeyType, string> keyPair = new Dictionary<KeyType, string>();
            System.Security.Cryptography.RSACryptoServiceProvider provider =
                new System.Security.Cryptography.RSACryptoServiceProvider();
            keyPair.Add(KeyType.PublicKey, provider.ToXmlString(false));
            keyPair.Add(KeyType.PrivateKey, provider.ToXmlString(true));
            return keyPair;
        }

        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="info">原数据</param>
        /// <param name="xmlPublicKey">公钥xml字符串</param>
        /// <returns>加密数据</returns>
        public byte[] Encrypt(byte[] info, string xmlPublicKey)
        {
            var data = new byte[0];
            System.Security.Cryptography.RSACryptoServiceProvider provider =
                new System.Security.Cryptography.RSACryptoServiceProvider();
            provider.FromXmlString(xmlPublicKey);
            data = provider.Encrypt(info, false);

            return data;
        }

        /// <summary>
        /// 私钥解密
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <param name="xmlPrivateKey">私钥xml字符串</param>
        /// <returns>解密数据</returns>
        public byte[] Decrypt(byte[] data, string xmlPrivateKey)
        {
            var info = new byte[0];
            System.Security.Cryptography.RSACryptoServiceProvider provider =
                new System.Security.Cryptography.RSACryptoServiceProvider();
            provider.FromXmlString(xmlPrivateKey);
            info = provider.Decrypt(data, false);

            return info;
        }
    }
}
