using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Rocket_League_Map_Loader.Helpers
{
    public class FileHelper
    {
        public static void CopyDirectory(string sourceDirectory, string destinationDirectory, bool overwrite)
        {
            foreach(var file in Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories))
            {
                var destinationFile = file.Replace(sourceDirectory, destinationDirectory);
                var destinationDir = destinationFile.Substring(0, destinationFile.LastIndexOf("\\"));

                if(!Directory.Exists(destinationDir))
                    Directory.CreateDirectory(destinationDir);

                File.Copy(file, destinationFile, overwrite);
            }
        }

        public static void MoveFile(string sourceFilePath, string destinationDirectory, bool backup = false, string backupDirectory = null)
        {
            var filename = Path.GetFileName(sourceFilePath);

            if (backup)
                BackupFile(sourceFilePath, backupDirectory);

            File.Copy(sourceFilePath, Path.Combine(destinationDirectory, filename), true);
            File.Delete(sourceFilePath);
        }

        public static void BackupFile(string sourceFilePath, string backupDirectory)
        {
            var filename = Path.GetFileName(sourceFilePath);
            var backupFileName = Path.Combine(backupDirectory, filename);

            if(!Directory.Exists(backupDirectory))
                Directory.CreateDirectory(backupDirectory);

            if (!File.Exists(backupFileName))
                File.Copy(sourceFilePath, backupFileName);
        }

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
                    if (!zipEntry.IsFile)
                        continue;

                    var entryFileName = zipEntry.Name;
                    var buffer = new byte[4096];
                    var zipStream = file.GetInputStream(zipEntry);
                    var fullZipToPath = Path.Combine(outputFolder, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);

                    if (!String.IsNullOrEmpty(directoryName)) 
                        Directory.CreateDirectory(directoryName);

                    using (var streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
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

        public static void ExtractZipFile(Stream stream, string password, string outputFolder)
        {
            ZipFile file = null;

            try
            {
                file = new ZipFile(stream);

                if (!String.IsNullOrEmpty(password))
                    file.Password = password;

                foreach (ZipEntry zipEntry in file)
                {
                    if (!zipEntry.IsFile)
                        continue;

                    var entryFileName = zipEntry.Name;
                    var buffer = new byte[4096];
                    var zipStream = file.GetInputStream(zipEntry);
                    var fullZipToPath = Path.Combine(outputFolder, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);

                    if (!String.IsNullOrEmpty(directoryName))
                        Directory.CreateDirectory(directoryName);

                    using (var streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
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

        public static string GetFileDirectory(string filePath) => filePath.Substring(0, filePath.LastIndexOf("\\"));

        public static string FindMapFile(string directory) =>
            FindAllMapFiles(directory).FirstOrDefault();

        public static string FindMapInfo(string directory) => 
            Directory.GetFiles(directory, "info.json", SearchOption.AllDirectories).FirstOrDefault();

        public static string FindExtraMapInfo(string directory) =>
            Directory.GetFiles(directory, "extra-info.json", SearchOption.AllDirectories).FirstOrDefault();

        public static string FindMapPreview(string directory) =>
            Directory.GetFiles(directory, "*.jpg", SearchOption.AllDirectories)
                .Union(Directory.GetFiles(directory, "*.png", SearchOption.AllDirectories)).FirstOrDefault();

        public static List<string> FindAllMapFiles(string directory) =>
            Directory.GetFiles(directory, "*.upk", SearchOption.AllDirectories)
                .Union(Directory.GetFiles(directory, "*.udk", SearchOption.AllDirectories)).ToList();
    }
}
