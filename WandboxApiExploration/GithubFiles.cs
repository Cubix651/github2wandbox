using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;
using Xunit.Abstractions;
using Github2Wandbox.Models.Github;

namespace WandboxApiExploration
{
    public class GithubFiles
    {
        string execute(string url, string userAgent)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            return client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
        }

        [Fact]
        void gets_files_in_directory()
        {
            string response = execute("https://api.github.com/repos/Cubix651/blog-examples/contents/concepts", "Cubix651");
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;
            var files = JsonConvert.DeserializeObject<List<ContentResponse>>(response, settings);
            Assert.Single(files);
        }

        [Fact]
        void gets_file_content()
        {
            string url = "https://raw.githubusercontent.com/Cubix651/blog-examples/master/concepts/concepts.cpp";
            string content = execute(url, "Cubix651");
            Assert.NotNull(content);
        }
    }
}
