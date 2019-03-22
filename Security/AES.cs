using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class AES
    {
        //public AES()
        //{

        //}
        //public AES(byte[] key)
        //{

        //}
        public int IVLenght
        {
            get
            {
                using (System.Security.Cryptography.AesCryptoServiceProvider
                   provider = new System.Security.Cryptography.AesCryptoServiceProvider())
                {
                    return provider.BlockSize / 8;
                }
            }
        }

        public byte[] Encrypt(byte[] info, byte[] key, byte[] iv)
        {
            using (System.Security.Cryptography.AesCryptoServiceProvider
                provider = new System.Security.Cryptography.AesCryptoServiceProvider())
            {
                var encryptor = provider.CreateEncryptor(key, iv);
                return encryptor.TransformFinalBlock(info, 0, info.Length);
            }
        }

        public byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (System.Security.Cryptography.AesCryptoServiceProvider
                provider = new System.Security.Cryptography.AesCryptoServiceProvider())
            {
                var decryptor = provider.CreateDecryptor(key, iv);
                return decryptor.TransformFinalBlock(data, 0, data.Length);
            }
        }
    }
}
