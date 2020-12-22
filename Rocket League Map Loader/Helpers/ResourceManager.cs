using System;
using System.IO;
using System.Reflection;

namespace Rocket_League_Map_Loader.Helpers
{
    public class ResourceManager
    {
        public static Stream LoadResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

#if DEBUG
            var resources = assembly.GetManifestResourceNames();

            foreach (var resource in resources)
                Console.WriteLine(resource);
#endif

            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}
