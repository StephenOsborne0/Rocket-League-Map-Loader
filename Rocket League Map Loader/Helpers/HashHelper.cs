using System;
using System.IO;
using System.Security.Cryptography;

namespace RL_Map_Loader.Helpers
{
    public class HashHelper
    {
        public static string GenerateHash(string filepath)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filepath))
                    {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "");
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
                //throw;
            }
        }
    }
}
