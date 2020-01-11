using System.IO;

namespace Github2WandboxIntegrationTests
{
    public class Resources
    {
        public static string Get(string path)
        {
            return File.ReadAllText($"Resources/{path}");
        }
    }
}