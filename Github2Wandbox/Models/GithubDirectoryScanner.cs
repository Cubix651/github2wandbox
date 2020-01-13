using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Github2Wandbox.Models.Github;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Github2Wandbox.Models
{
    public class GithubDirectoryScanner : IGithubScanner
    {
        HttpClient httpClient;
        JsonSerializerSettings jsonSettings;

        public static string UserAgent { get; } = "Github2Wandbox";

        public GithubDirectoryScanner()
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

        public async Task<SourceFiles> GetSourceFilesAsync(GithubDirectoryDescription description)
        {
            int limes = description.MainPath.LastIndexOf('/');
            string mainDirectory = description.MainPath.Substring(0, limes);
            string mainFile = description.MainPath.Substring(limes + 1);
            string apiUrl = $"https://api.github.com/repos/{description.Owner}/{description.Repository}/contents/{mainDirectory}";
            string response = await GetHttpAsync(apiUrl);
            var files = JsonConvert.DeserializeObject<List<ContentResponse>>(response, jsonSettings);
            var allSourceFiles = files
                .Select(f => new SourceFile
                {
                    File = f.Name,
                    Code = GetHttpAsync(f.DownloadUrl).Result
                });
            return new SourceFiles
            {
                Code = allSourceFiles.Single(f => f.File == mainFile).Code,
                Codes = allSourceFiles.Where(f => f.File != mainFile).ToList()
            };
        }
    }
}
