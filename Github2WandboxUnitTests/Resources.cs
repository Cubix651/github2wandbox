using System.IO;

namespace Github2WandboxUnitTests
{
    public class Resources
    {
        public static string Get(string path)
        {
            return File.ReadAllText($"Resources/{path}");
        }
    }
}