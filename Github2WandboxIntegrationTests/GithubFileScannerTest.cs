using Xunit;
using Github2Wandbox.Models;

namespace Github2WandboxIntegrationTests
{
    public class GithubFileScannerTest
    {
        GithubFileScanner githubFileScanner = new GithubFileScanner();

        [Fact]
        public void should_get_single_file_content()
        {
            var directoryDescription = new GithubDirectoryDescription
            {
                Owner = "Cubix651",
                Repository = "github2wandbox-testrepo",
                MainPath = "singlefile-examples/a.cpp",
            };

            var sourceFiles = githubFileScanner.GetSourceFiles(directoryDescription);

            Assert.Null(sourceFiles.Codes);
            Assert.Equal(Resources.Get("singlefile/a.cpp"), sourceFiles.Code);
        }
    }
}
