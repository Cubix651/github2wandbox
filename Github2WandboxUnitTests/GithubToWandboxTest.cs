using Xunit;
using FakeItEasy;
using Github2Wandbox.Models;
using Github2Wandbox.Repository;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace Github2WandboxUnitTests
{
    public class GithubToWandboxTest : IDisposable
    {
        PublicationsContext context;
        GithubDirectoryCommitChecker githubCommitChecker;
        IGithubScanner githubScanner;
        WandboxPublisher wandboxPublisher;
        GithubToWandbox instance;

        public GithubToWandboxTest()
        {
            var contextOptions = new DbContextOptionsBuilder<PublicationsContext>()
                .UseInMemoryDatabase(databaseName: "test_databse")
                .Options;
            context = new PublicationsContext(contextOptions);
            githubCommitChecker = A.Fake<GithubDirectoryCommitChecker>();
            githubScanner = A.Fake<IGithubScanner>();
            wandboxPublisher = A.Fake<WandboxPublisher>();
            instance = new GithubToWandbox(context, githubCommitChecker, githubScanner, wandboxPublisher);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        [Fact]
        public async Task should_return_url_from_database_when_matching_record_exists()
        {
            var publication = new Publication
            {
                CommitSha = "commithash",
                MainPath = "a/b/c",
                Owner = "owner",
                Repository = "repo",
                Url = "https://wandbox.org/permlink/wandboxHash"
            };
            context.Publications.Add(publication);
            context.SaveChanges();

            var githubDirectoryDescription = new GithubDirectoryDescription
            {
                MainPath = "a/b/c",
                Owner = "owner",
                Repository = "repo"
            };

            A.CallTo(() => githubCommitChecker.GetCommitShaAsync(githubDirectoryDescription))
                .Returns("commithash");

            var transformationDescription = new TransformationDescription
            {
                GithubDirectoryDescription = githubDirectoryDescription,
                WandboxOptions = null
            };

            var url = await instance.TransformAsync(transformationDescription);

            A.CallTo(() => githubScanner.GetSourceFilesAsync(A<GithubDirectoryDescription>._))
                .MustNotHaveHappened();
            A.CallTo(() => wandboxPublisher.PublishAsync(A<SourceFiles>._, A<WandboxOptions>._))
                .MustNotHaveHappened();
            Assert.Equal("https://wandbox.org/permlink/wandboxHash", url);
        }

    }
}
