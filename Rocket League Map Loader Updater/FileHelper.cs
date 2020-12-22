using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Rocket_League_Map_Loader_Updater
{
    public class FileHelper
    {
        public static void ExtractZipFile(string zipFilePath, string password, string outputFolder)
        {
            ZipFile file = null;

            try
            {
                var fs = File.OpenRead(zipFilePath);
                file = new ZipFile(fs);

                if (!String.IsNullOrEmpty(password))
                    file.Password = password;

                foreach (ZipEntry zipEntry in file)
                {
                    try
                    {
                        if(!zipEntry.IsFile)
                            continue;

                        var entryFileName = zipEntry.Name;
                        Console.WriteLine($"Extracting {zipEntry.Name}");
                        var buffer = new byte[4096];
                        var zipStream = file.GetInputStream(zipEntry);
                        var fullZipToPath = Path.Combine(outputFolder, entryFileName);
                        var directoryName = Path.GetDirectoryName(fullZipToPath);

                        if(!String.IsNullOrEmpty(directoryName))
                            Directory.CreateDirectory(directoryName);

                        using(var streamWriter = File.Create(fullZipToPath))
                        {
                            StreamUtils.Copy(zipStream, streamWriter, buffer);
                        }
                    }
                    catch
                    {
                        //Added try to avoid updater overwriting it's own files
                    }
                }
            }
            finally
            {
                if (file != null)
                {
                    file.IsStreamOwner = true;
                    file.Close();
                }
            }
        }
    }
}
