using Xunit;
using FakeItEasy;
using Github2Wandbox.Models.Github;
using System.Linq;
using System.Threading.Tasks;
using Github2Wandbox.Models.Communication;

namespace Github2WandboxUnitTests
{
    public class GithubFileScannerTest
    {
        IHttpClient httpClient;
        GithubFileScanner githubFileScanner;

        public GithubFileScannerTest()
        {
            httpClient = A.Fake<IHttpClient>();
            githubFileScanner = new GithubFileScanner(httpClient);
        }

        [Fact]
        public async Task should_get_single_file_content()
        {
            string url = "https://raw.githubusercontent.com/Cubix651/github2wandbox-testrepo/master/singlefile-examples/a.cpp";
            A.CallTo(() => httpClient.GetAsync(url))
                .Returns(Task.FromResult("Test content"));

            var directoryDescription = new GithubDirectoryDescription
            {
                Owner = "Cubix651",
                Repository = "github2wandbox-testrepo",
                MainPath = "singlefile-examples/a.cpp",
            };

            var sourceFiles = await githubFileScanner.GetSourceFilesAsync(directoryDescription);

            Assert.Null(sourceFiles.Codes);
            Assert.Equal("Test content", sourceFiles.Code);
        }
    }
}
