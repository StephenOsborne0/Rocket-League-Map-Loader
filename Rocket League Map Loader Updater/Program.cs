using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Threading.Thread;

namespace Rocket_League_Map_Loader_Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args == null || args.Length != 1)
                return;

            WaitForClose();

            var updateZip = args[0];
            Update(updateZip);

            Console.WriteLine();
            Console.Write("Launching Rocket League Map Loader...");
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rocket League Map Loader.exe");
            Process.Start(filePath);

#if (DEBUG)
            Console.ReadLine();
#endif
        }

        private static void WaitForClose()
        {
            var process = Process.GetProcessesByName("Rocket League Map Loader.exe").FirstOrDefault();

            if(process == null) 
                return;

            while (!process.HasExited)
            {
                Console.WriteLine("Waiting for Rocket League Map Loader to close...");
                Sleep(1000);
            }
        }

        private static void Update(string updateFile)
        {
            Console.WriteLine("Updating");
            Console.WriteLine($"Extracting {updateFile}...");
            FileHelper.ExtractZipFile(updateFile, null, AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("Update completed.");
        }
    }
}
