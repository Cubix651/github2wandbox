using Xunit;
using FakeItEasy;
using Github2Wandbox.Models.Github;
using System.Linq;
using System.Threading.Tasks;
using Github2Wandbox.Models.Communication;

namespace Github2WandboxUnitTests
{
    public class GithubDirectoryCommitCheckerTest
    {
        IHttpClient httpClient;
        GithubDirectoryCommitChecker githubDirectoryCommitChecker;

        public GithubDirectoryCommitCheckerTest()
        {
            httpClient = A.Fake<IHttpClient>();
            githubDirectoryCommitChecker = new GithubDirectoryCommitChecker(httpClient);
        }

        [Fact]
        public async Task should_get_correct_commit_hash()
        {
            string url = "https://api.github.com/repos/Cubix651/github2wandbox-testrepo/commits?path=multifile-example"     ;
            A.CallTo(() => httpClient.GetAsync(url))
                .Returns(Task.FromResult(Resources.Get("directory_commit_response.json")));

            var directoryDescription = new GithubDirectoryDescription
            {
                Owner = "Cubix651",
                Repository = "github2wandbox-testrepo",
                MainPath = "multifile-example/main.cpp",
            };

            string commitHash = await githubDirectoryCommitChecker.GetCommitShaAsync(directoryDescription);

            Assert.Equal("62a4fcc7191026dc299f88a72a4ef82939aa5611", commitHash);
        }
    }
}
