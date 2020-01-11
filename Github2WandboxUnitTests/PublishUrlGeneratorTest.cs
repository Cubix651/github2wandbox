using System;
using Xunit;
using Github2Wandbox.Models;
using Github2Wandbox.ViewModels;

namespace Github2WandboxUnitTests
{
    public class PublishUrlGeneratorTest
    {
        PublishUrlGenerator publishUrlGenerator;

        public PublishUrlGeneratorTest()
        {
            publishUrlGenerator = new PublishUrlGenerator();
        }

        [Fact]
        public void should_generate_proper_url()
        {
            var optiones = new OptionsViewModel
            {
                owner = "Cubix651",
                repository = "github2wandbox",
                main_path = "sources/main.cpp",
                compiler_standard = "abc"
            };
            var url = publishUrlGenerator.Generate(optiones);

            Assert.Equal("Publish/Cubix651/github2wandbox/sources/main.cpp?compiler_standard=abc", url);
        }

        [Fact]
        public void should_encode_compiler_standard()
        {
            var optiones = new OptionsViewModel
            {
                owner = "a",
                repository = "b",
                main_path = "c/d.cpp",
                compiler_standard = "c++2a"
            };

            var url = publishUrlGenerator.Generate(optiones);

            Assert.Equal("Publish/a/b/c/d.cpp?compiler_standard=c%2B%2B2a", url);
        }
    }
}
