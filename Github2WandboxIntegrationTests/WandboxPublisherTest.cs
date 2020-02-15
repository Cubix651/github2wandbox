using Xunit;
using Github2Wandbox.Models.Common;
using Github2Wandbox.Models.Wandbox;
using System.Linq;
using System.Threading.Tasks;
using Github2Wandbox.Models.Communication;

namespace Github2WandboxIntegrationTests
{
    public class WandboxPublisherTest
    {
        IHttpClient httpClient;
        WandboxPublisher wandboxPublisher;

        public WandboxPublisherTest()
        {
            httpClient = new HttpClient();
            httpClient.AddUserAgent("Github2WandboxIntegrationTests");
            wandboxPublisher = new WandboxPublisher(httpClient);
        }

        [Fact]
        public async Task should_publish_single_file_successfully()
        {
            var wandboxOptions = new WandboxOptions { CompilerStandard = "c++2a" };
            var sourceFiles = new SourceFiles { Code = Resources.Get("singlefile/a.cpp") };

            string url = await wandboxPublisher.PublishAsync(sourceFiles, wandboxOptions);

            Assert.StartsWith("https://wandbox.org", url);
        }

        [Fact]
        public async Task should_publish_multi_files_successfully()
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

            string url = await wandboxPublisher.PublishAsync(sourceFiles, wandboxOptions);

            Assert.StartsWith("https://wandbox.org", url);
        }
    }
}
