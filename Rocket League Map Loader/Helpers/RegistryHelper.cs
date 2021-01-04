using System;
using System.Linq;
using Microsoft.Win32;
using System.IO;

namespace RL_Map_Loader.Helpers
{
    public class RegistryHelper
    {
        public static string FindExecutableFilePath(string executable)
        {
            var executablesKeyPath ="SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\AppCompatFlags\\Compatibility Assistant\\Store";
            var executablesKey = Registry.CurrentUser.OpenSubKey(executablesKeyPath);
            return executablesKey?.GetValueNames().FirstOrDefault(x => x.Contains(executable) && File.Exists(x));
        }

        public static string FindExecutableDirectoryPath(string executable)
        {
            var path = FindExecutableFilePath(executable);

            if(path != null)
            {
                var index = path.IndexOf(executable, StringComparison.Ordinal) + executable.Length;

                if (index > 0 && index < path.Length)
                    return path.Substring(0, index);
            }

            return null;
        }

        public static bool IsExecutableInstalled(string executable) => FindExecutableFilePath(executable) != null;
    }
}
