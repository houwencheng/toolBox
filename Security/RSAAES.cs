using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class RSAAES
    {
        private byte[] iv = new byte[2] { 0, 1 };

        /// <summary>
        /// 创建一个由RAS加密的密码
        /// </summary>
        /// <param name="xmlPublicKey"></param>
        /// <returns></returns>
        public byte[] CreateKeyByRSAEncrypt(string xmlPublicKey)
        {
            var guid = Guid.NewGuid().ToByteArray();
            RSA rsa = new RSA();
            var key = rsa.Encrypt(guid, xmlPublicKey);
            return key;
        }

        public byte[] Encrypt(byte[] info, byte[] keyInRSAEncrypt, string xmlPrivateKey)
        {
            RSA rsa = new RSA();
            var key = rsa.Decrypt(keyInRSAEncrypt, xmlPrivateKey);
            AES aes = new AES();
            List<byte> ivList = new List<byte>(iv);
            while (ivList.Count < aes.IVLenght)
            {
                ivList.Add(new byte());
            }
            var data = aes.Encrypt(info, key, ivList.ToArray());
            return data;
        }

        public byte[] Decrypt(byte[] data, byte[] keyInRSAEncrypt, string xmlPrivateKey)
        {
            RSA rsa = new RSA();
            var key = rsa.Decrypt(keyInRSAEncrypt, xmlPrivateKey);
            AES aes = new AES();
            List<byte> ivList = new List<byte>(iv);
            while (ivList.Count < aes.IVLenght)
            {
                ivList.Add(new byte());
            }
            var info = aes.Decrypt(data, key, ivList.ToArray());
            return info;
        }
    }
}
