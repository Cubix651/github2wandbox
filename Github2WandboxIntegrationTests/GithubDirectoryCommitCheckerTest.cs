using Xunit;
using Github2Wandbox.Models.Github;
using System.Threading.Tasks;
using Github2Wandbox.Models.Communication;

namespace Github2WandboxIntegrationTests
{
    public class GithubDirectoryCommitCheckerTest
    {
        IHttpClient httpClient;
        GithubDirectoryCommitChecker githubDirectoryCommitChecker;

        public GithubDirectoryCommitCheckerTest()
        {
            httpClient = new HttpClient();
            httpClient.AddUserAgent("Github2WandboxIntegrationTests");
            githubDirectoryCommitChecker = new GithubDirectoryCommitChecker(httpClient);
        }

        [Fact]
        public async Task should_get_correct_commit_hash()
        {
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
