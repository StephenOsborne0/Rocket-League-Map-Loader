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

            var updateZip = args[1];
            Update(updateZip);

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rocket League Map Loader.exe");
            Process.Start(filePath);
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

        private static void Update(string updateFile) => FileHelper.ExtractZipFile(updateFile, null, AppDomain.CurrentDomain.BaseDirectory);
    }
}
