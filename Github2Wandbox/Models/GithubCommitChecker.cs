using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Github2Wandbox.Models.Github;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Github2Wandbox.Models
{
    public class GithubDirectoryCommitChecker
    {
        HttpClient httpClient;
        JsonSerializerSettings jsonSettings;

        public static string UserAgent { get; } = "Github2Wandbox";

        public GithubDirectoryCommitChecker()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);

            jsonSettings = new JsonSerializerSettings();
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        private async Task<string> GetHttpAsync(string url)
        {
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public virtual async Task<string> GetCommitShaAsync(GithubDirectoryDescription description)
        {
            string mainDirectory = Path.GetDirectoryName(description.MainPath);
            string escapedMainDirectory = WebUtility.UrlEncode(mainDirectory);
            string apiUrl = $"https://api.github.com/repos/{description.Owner}/" +
                $"{description.Repository}/commits?path={escapedMainDirectory}";
            string response = await GetHttpAsync(apiUrl);
            var commits = JsonConvert.DeserializeObject<List<CommitResponse>>(response, jsonSettings);
            var commit = commits.First();
            return commit.Sha;
        }
    }
}
