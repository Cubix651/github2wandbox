using Xunit;
using FakeItEasy;
using Github2Wandbox.Models;
using Github2Wandbox.Repository;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using Github2Wandbox.Models.Github;
using Github2Wandbox.Models.Wandbox;
using Github2Wandbox.Models.Common;

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

        [Fact]
        public async Task should_return_new_url_when_repo_updated()
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
                .Returns("commithash2");

            var sourceFiles = A.Fake<SourceFiles>();
            A.CallTo(() => githubScanner.GetSourceFilesAsync(githubDirectoryDescription))
                .Returns(sourceFiles);

            var wandboxOptions = A.Fake<WandboxOptions>();
            A.CallTo(() => wandboxPublisher.PublishAsync(sourceFiles, wandboxOptions))
                .Returns("https://wandbox.org/permlink/wandboxHash2");

            var transformationDescription = new TransformationDescription
            {
                GithubDirectoryDescription = githubDirectoryDescription,
                WandboxOptions = wandboxOptions
            };

            var url = await instance.TransformAsync(transformationDescription);

            Assert.Equal("https://wandbox.org/permlink/wandboxHash2", url);
        }

        [Fact]
        public async Task should_return_new_url_when_not_in_database()
        {
            var githubDirectoryDescription = new GithubDirectoryDescription
            {
                MainPath = "a/b/c",
                Owner = "owner",
                Repository = "repo"
            };
            A.CallTo(() => githubCommitChecker.GetCommitShaAsync(githubDirectoryDescription))
                .Returns("commithash");

            var sourceFiles = A.Fake<SourceFiles>();
            A.CallTo(() => githubScanner.GetSourceFilesAsync(githubDirectoryDescription))
                .Returns(sourceFiles);

            var wandboxOptions = A.Fake<WandboxOptions>();
            A.CallTo(() => wandboxPublisher.PublishAsync(sourceFiles, wandboxOptions))
                .Returns("https://wandbox.org/permlink/wandboxHash");

            var transformationDescription = new TransformationDescription
            {
                GithubDirectoryDescription = githubDirectoryDescription,
                WandboxOptions = wandboxOptions
            };

            var url = await instance.TransformAsync(transformationDescription);

            Assert.Equal("https://wandbox.org/permlink/wandboxHash", url);
        }

        [Fact]
        public async Task should_return_new_url_when_main_path_mismatched()
        {
            var publication = new Publication
            {
                CommitSha = "commithash",
                MainPath = "a/b/d",
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

            var sourceFiles = A.Fake<SourceFiles>();
            A.CallTo(() => githubScanner.GetSourceFilesAsync(githubDirectoryDescription))
                .Returns(sourceFiles);

            var wandboxOptions = A.Fake<WandboxOptions>();
            A.CallTo(() => wandboxPublisher.PublishAsync(sourceFiles, wandboxOptions))
                .Returns("https://wandbox.org/permlink/wandboxHash");

            var transformationDescription = new TransformationDescription
            {
                GithubDirectoryDescription = githubDirectoryDescription,
                WandboxOptions = wandboxOptions
            };

            var url = await instance.TransformAsync(transformationDescription);

            Assert.Equal("https://wandbox.org/permlink/wandboxHash", url);
        }

        [Fact]
        public async Task should_return_new_url_when_owner_mismatched()
        {
            var publication = new Publication
            {
                CommitSha = "commithash",
                MainPath = "a/b/c",
                Owner = "owner2",
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

            var sourceFiles = A.Fake<SourceFiles>();
            A.CallTo(() => githubScanner.GetSourceFilesAsync(githubDirectoryDescription))
                .Returns(sourceFiles);

            var wandboxOptions = A.Fake<WandboxOptions>();
            A.CallTo(() => wandboxPublisher.PublishAsync(sourceFiles, wandboxOptions))
                .Returns("https://wandbox.org/permlink/wandboxHash");

            var transformationDescription = new TransformationDescription
            {
                GithubDirectoryDescription = githubDirectoryDescription,
                WandboxOptions = wandboxOptions
            };

            var url = await instance.TransformAsync(transformationDescription);

            Assert.Equal("https://wandbox.org/permlink/wandboxHash", url);
        }

        [Fact]
        public async Task should_return_new_url_when_repo_mismatched()
        {
            var publication = new Publication
            {
                CommitSha = "commithash",
                MainPath = "a/b/c",
                Owner = "owner",
                Repository = "repo2",
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

            var sourceFiles = A.Fake<SourceFiles>();
            A.CallTo(() => githubScanner.GetSourceFilesAsync(githubDirectoryDescription))
                .Returns(sourceFiles);

            var wandboxOptions = A.Fake<WandboxOptions>();
            A.CallTo(() => wandboxPublisher.PublishAsync(sourceFiles, wandboxOptions))
                .Returns("https://wandbox.org/permlink/wandboxHash");

            var transformationDescription = new TransformationDescription
            {
                GithubDirectoryDescription = githubDirectoryDescription,
                WandboxOptions = wandboxOptions
            };

            var url = await instance.TransformAsync(transformationDescription);

            Assert.Equal("https://wandbox.org/permlink/wandboxHash", url);
        }
    }
}
