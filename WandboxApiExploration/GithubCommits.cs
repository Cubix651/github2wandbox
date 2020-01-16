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
    public class GithubCommits
    {
        string execute(string url, string userAgent)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            return client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
        }

        [Fact]
        void gets_commits_for_directory()
        {
            string response = execute("https://api.github.com/repos/Cubix651/github2wandbox-testrepo/commits?path=multifile-example/", "Cubix651");
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;
            var commits = JsonConvert.DeserializeObject<List<CommitResponse>>(response, settings);
            var commit = commits[0];
            Assert.Equal("62a4fcc7191026dc299f88a72a4ef82939aa5611", commit.Sha);
        }
    }
}
