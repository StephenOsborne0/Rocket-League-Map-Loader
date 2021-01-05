using System;

namespace UPK_File_Extractor
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args == null || args.Length < 1)
                return;

            var filepath = args[0];

            var upkFile = new UPKFile(filepath);
            upkFile.ExtractAssets();
            Console.ReadLine();
        }
    }
}
