using Xunit;
using Github2Wandbox.Models;
using System.Linq;

namespace Github2WandboxIntegrationTests
{
    public class WandboxPublisherTest
    {
        WandboxPublisher wandboxPublisher = new WandboxPublisher();

        [Fact]
        public void should_publish_single_file_successfully()
        {
            var wandboxOptions = new WandboxOptions { CompilerStandard = "c++2a" };
            var sourceFiles = new SourceFiles { Code = Resources.Get("singlefile/a.cpp") };

            string url = wandboxPublisher.Publish(sourceFiles, wandboxOptions);

            Assert.StartsWith("https://wandbox.org", url);
        }

        [Fact]
        public void should_publish_multi_files_successfully()
        {
            var wandboxOptions = new WandboxOptions { CompilerStandard = "c++2a" };
            string directory = "multifile";
            var sourceFiles = new SourceFiles
            {
                Code = Resources.Get($"{directory}/main.cpp"),
                Codes = new[] { "a.cpp", "a.h", "b.cpp", "b.h" }
                    .Select(filename => new SourceFile
                    {
                        File = filename,
                        Code = Resources.Get($"{directory}/{filename}")
                    }).ToList()
            };

            string url = wandboxPublisher.Publish(sourceFiles, wandboxOptions);

            Assert.StartsWith("https://wandbox.org", url);
        }
    }
}
