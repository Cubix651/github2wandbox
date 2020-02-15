using Xunit;
using FakeItEasy;
using Github2Wandbox.Models.Github;
using System.Linq;
using System.Threading.Tasks;
using Github2Wandbox.Models.Communication;

namespace Github2WandboxUnitTests
{
    public class GithubDirectoryScannerTest
    {
        IHttpClient httpClient;
        GithubDirectoryScanner githubDirectoryScanner;

        public GithubDirectoryScannerTest()
        {
            httpClient = A.Fake<IHttpClient>();
            githubDirectoryScanner = new GithubDirectoryScanner(httpClient);
        }

        [Fact]
        public async Task should_get_multifile_content()
        {
            string apiDirectoryUrl = "https://api.github.com/repos/Cubix651/github2wandbox-testrepo/contents/multifile-example";
            A.CallTo(() => httpClient.GetAsync(apiDirectoryUrl))
                .Returns(Task.FromResult(Resources.Get("multifile_response.json")));

            foreach (var fileName in new[] { "a.cpp", "a.h", "b.cpp", "b.h", "main.cpp" })
            {
                string url = $"https://raw.githubusercontent.com/Cubix651/github2wandbox-testrepo/master/multifile-example/{fileName}";
                A.CallTo(() => httpClient.GetAsync(url))
                    .Returns(Task.FromResult($"Content of {fileName}"));
            }

            var directoryDescription = new GithubDirectoryDescription
            {
                Owner = "Cubix651",
                Repository = "github2wandbox-testrepo",
                MainPath = "multifile-example/main.cpp",
            };

            var sourceFiles = await githubDirectoryScanner.GetSourceFilesAsync(directoryDescription);

            Assert.Equal("Content of main.cpp", sourceFiles.Code);
            Assert.Equal(new[] { "a.cpp", "a.h", "b.cpp", "b.h" },
                sourceFiles.Codes.Select(c => c.File).OrderBy(f => f));
            foreach(var code in sourceFiles.Codes)
                Assert.Equal($"Content of {code.File}", code.Code);
        }
    }
}
