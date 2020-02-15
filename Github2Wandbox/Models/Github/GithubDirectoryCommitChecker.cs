using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Github2Wandbox.Models.Communication;
using Github2Wandbox.Models.Github.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Github2Wandbox.Models.Github
{
    public class GithubDirectoryCommitChecker
    {
        IHttpClient httpClient;
        JsonSerializerSettings jsonSettings;

        public GithubDirectoryCommitChecker(IHttpClient httpClient)
        {
            this.httpClient = httpClient;

            jsonSettings = new JsonSerializerSettings();
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public virtual async Task<string> GetCommitShaAsync(GithubDirectoryDescription description)
        {
            string mainDirectory = Path.GetDirectoryName(description.MainPath);
            string escapedMainDirectory = WebUtility.UrlEncode(mainDirectory);
            string apiUrl = $"https://api.github.com/repos/{description.Owner}/" +
                $"{description.Repository}/commits?path={escapedMainDirectory}";
            string response = await httpClient.GetAsync(apiUrl);
            var commits = JsonConvert.DeserializeObject<List<CommitResponse>>(response, jsonSettings);
            var commit = commits.First();
            return commit.Sha;
        }
    }
}
