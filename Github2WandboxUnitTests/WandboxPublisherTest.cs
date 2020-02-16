using Xunit;
using FakeItEasy;
using Github2Wandbox.Models.Common;
using Github2Wandbox.Models.Wandbox;
using System.Threading.Tasks;
using Github2Wandbox.Models.Communication;
using Newtonsoft.Json.Linq;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Github2WandboxUnitTests
{
    public class WandboxPublisherTest
    {
        private readonly string Url = "https://wandbox.org/api/compile.json";

        IHttpClient httpClient;
        WandboxPublisher wandboxPublisher;

        public WandboxPublisherTest()
        {
            httpClient = A.Fake<IHttpClient>();
            wandboxPublisher = new WandboxPublisher(httpClient);
        }

        private static Expression<Func<string, bool>> Jsonify(Expression<Func<JObject, bool>> expression)
        {
            Expression<Func<string, JObject>> substitution = str => JObject.Parse(str);

            return Expression.Lambda<Func<string, bool>>(
                Expression.Invoke(expression, Expression.Invoke(substitution, substitution.Parameters)),
                substitution.Parameters);
        }

        private string CreateWandboxResponse(string id)
        {
            return JObject.FromObject(new
            {
                permlink = id,
                status = "ok",
                url = $"https://wandbox.org/permlink/{id}"
            }).ToString();
        }

        [Fact]
        public async Task should_publish_with_gcc_compiler()
        {
            var wandboxOptions = new WandboxOptions { };
            var sourceFiles = new SourceFiles { };

            A.CallTo(() => httpClient.PostAsync(A<string>._, A<string>._, A<string>._))
                .Returns(Task.FromResult(CreateWandboxResponse("testHash")));

            string url = await wandboxPublisher.PublishAsync(sourceFiles, wandboxOptions);

            A.CallTo(() => httpClient.PostAsync(Url,
                    A<string>.That.Matches(Jsonify(arg =>
                        arg["compiler"].ToString() == "gcc-head" &&
                        arg["save"].Value<bool>() == true)),
                    "application/json"))
                .MustHaveHappenedOnceExactly();
            Assert.Equal("https://wandbox.org/permlink/testHash", url);
        }

        [Fact]
        public async Task should_publish_successfully_when_single_file_provided()
        {
            var wandboxOptions = new WandboxOptions { };
            var sourceFiles = new SourceFiles { Code = "Content of main file" };

            A.CallTo(() => httpClient.PostAsync(A<string>._, A<string>._, A<string>._))
                .Returns(Task.FromResult(CreateWandboxResponse("testHash")));

            string url = await wandboxPublisher.PublishAsync(sourceFiles, wandboxOptions);

            A.CallTo(() => httpClient.PostAsync(Url,
                    A<string>.That.Matches(Jsonify(arg =>
                        arg["code"].ToString() == "Content of main file")),
                    "application/json"))
                .MustHaveHappenedOnceExactly();
            Assert.Equal("https://wandbox.org/permlink/testHash", url);
        }

        [Fact]
        public async Task should_pass_compiler_standard_when_provided()
        {
            var wandboxOptions = new WandboxOptions { CompilerStandard = "c++2a" };
            var sourceFiles = new SourceFiles { };

            A.CallTo(() => httpClient.PostAsync(A<string>._, A<string>._, A<string>._))
                .Returns(Task.FromResult(CreateWandboxResponse("testHash")));

            string url = await wandboxPublisher.PublishAsync(sourceFiles, wandboxOptions);

            A.CallTo(() => httpClient.PostAsync(Url,
                    A<string>.That.Matches(Jsonify(arg =>
                        arg["options"].ToString() == "c++2a")),
                    "application/json"))
                .MustHaveHappenedOnceExactly();
            Assert.Equal("https://wandbox.org/permlink/testHash", url);
        }

        [Fact]
        public async Task should_publish_successfully_when_two_files_provided()
        {
            var wandboxOptions = new WandboxOptions { };
            var sourceFiles = new SourceFiles
            {
                Codes = new List<SourceFile>
                {
                    new SourceFile
                    {
                        File = "second.cpp",
                        Code = "Content of second file"
                    }
                }
            };

            A.CallTo(() => httpClient.PostAsync(A<string>._, A<string>._, A<string>._))
                .Returns(Task.FromResult(CreateWandboxResponse("testHash")));

            string url = await wandboxPublisher.PublishAsync(sourceFiles, wandboxOptions);

            A.CallTo(() => httpClient.PostAsync(Url,
                    A<string>.That.Matches(Jsonify(arg =>
                        arg["codes"][0]["file"].ToString() == "second.cpp" &&
                        arg["codes"][0]["code"].ToString() == "Content of second file")),
                    "application/json"))
                .MustHaveHappenedOnceExactly();
            Assert.Equal("https://wandbox.org/permlink/testHash", url);
        }

        [Fact]
        public async Task should_pass_compiler_options_raw_when_more_source_files_provided()
        {
            var wandboxOptions = new WandboxOptions { };
            var sourceFiles = new SourceFiles
            {
                Codes = new List<SourceFile>
                {
                    new SourceFile { File = "second.cpp" },
                    new SourceFile { File = "third.cpp" },
                    new SourceFile { File = "header.h" },
                }
            };

            A.CallTo(() => httpClient.PostAsync(A<string>._, A<string>._, A<string>._))
                .Returns(Task.FromResult(CreateWandboxResponse("testHash")));

            string url = await wandboxPublisher.PublishAsync(sourceFiles, wandboxOptions);

            A.CallTo(() => httpClient.PostAsync(Url,
                    A<string>.That.Matches(Jsonify(arg =>
                        arg["compiler-option-raw"].ToString() == "second.cpp\nthird.cpp")),
                    "application/json"))
                .MustHaveHappenedOnceExactly();
            Assert.Equal("https://wandbox.org/permlink/testHash", url);
        }

        [Fact]
        public async Task should_publish_successfully_when_three_files_provided()
        {
            var wandboxOptions = new WandboxOptions { };
            var sourceFiles = new SourceFiles
            {
                Codes = new List<SourceFile>
                {
                    new SourceFile
                    {
                        File = "second.cpp",
                        Code = "Content of second source"
                    },
                    new SourceFile
                    {
                        File = "second.h",
                        Code = "Content of second header"
                    }
                }
            };

            A.CallTo(() => httpClient.PostAsync(A<string>._, A<string>._, A<string>._))
                .Returns(Task.FromResult(CreateWandboxResponse("testHash")));

            string url = await wandboxPublisher.PublishAsync(sourceFiles, wandboxOptions);

            A.CallTo(() => httpClient.PostAsync(Url,
                    A<string>.That.Matches(Jsonify(arg =>
                        arg["codes"][0]["file"].ToString() == "second.cpp" &&
                        arg["codes"][0]["code"].ToString() == "Content of second source" &&
                        arg["codes"][1]["file"].ToString() == "second.h" &&
                        arg["codes"][1]["code"].ToString() == "Content of second header")),
                    "application/json"))
                .MustHaveHappenedOnceExactly();
            Assert.Equal("https://wandbox.org/permlink/testHash", url);
        }
    }
}
