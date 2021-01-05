using System;
using System.IO;
using System.Security.Cryptography;

namespace RL_Map_Loader.Helpers
{
    public class HashHelper
    {
        public static string GenerateSHA256HashFromFile(string filepath) => 
            GenerateHashFromFile(filepath, SHA256.Create());

        public static string GenerateMD5HashFromFile(string filepath) => 
            GenerateHashFromFile(filepath, new MD5CryptoServiceProvider());

        public static string GenerateHashFromFile(string filepath, HashAlgorithm hashAlgorithm)
        {
            if(!File.Exists(filepath) || hashAlgorithm == null)
                return null;

            var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read) { Position = 0 };
            var hash = BitConverter.ToString(hashAlgorithm.ComputeHash(fs)).Replace("-", string.Empty);
            fs.Close();
            return hash;
        }
    }
}
