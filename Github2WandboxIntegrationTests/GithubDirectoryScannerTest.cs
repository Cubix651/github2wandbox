using Xunit;
using Github2Wandbox.Models.Common;
using Github2Wandbox.Models.Github;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Github2Wandbox.Models.Communication;

namespace Github2WandboxIntegrationTests
{
    public class GithubDirectoryScannerTest
    {
        static readonly string Directory = "multifile";

        IHttpClient httpClient;
        GithubDirectoryScanner githubFileScanner;

        public GithubDirectoryScannerTest()
        {
            httpClient = new HttpClient();
            httpClient.AddUserAgent("Github2WandboxIntegrationTests");
            githubFileScanner = new GithubDirectoryScanner(httpClient);
        }

        [Fact]
        public async Task should_get_multifile_content()
        {
            var directoryDescription = new GithubDirectoryDescription
            {
                Owner = "Cubix651",
                Repository = "github2wandbox-testrepo",
                MainPath = "multifile-example/main.cpp",
            };

            var sourceFiles = await githubFileScanner.GetSourceFilesAsync(directoryDescription);

            Assert.Equal(Resources.Get($"{Directory}/main.cpp"), sourceFiles.Code);
            var expectedCodes = new[] { "a.cpp", "a.h", "b.cpp", "b.h" }
                .Select(filename => new SourceFile
                {
                    File = filename,
                    Code = Resources.Get($"{Directory}/{filename}")
                });
            Assert.Equal(expectedCodes, sourceFiles.Codes.OrderBy(sf => sf.File));
        }
    }
}
