using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    public class MD5
    {
        public string Compute(byte[] data)
        {
            System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create();
            byte[] md5Value = md5Hash.ComputeHash(data);
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < md5Value.Length; i++)
            {
                sBuilder.Append(md5Value[i].ToString("x2"));
            }
            string md5String = sBuilder.ToString();
            return md5String;
        }

        public string Compute(System.IO.Stream stream)
        {
            System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create();
            var md5Value = md5Hash.ComputeHash(stream);
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < md5Value.Length; i++)
            {
                sBuilder.Append(md5Value[i].ToString("x2"));
            }
            string md5String = sBuilder.ToString();
            return md5String;
        }
    }
}
