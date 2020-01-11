using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        private string GetHttp(string url)
        {
            return httpClient.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
        }

        public SourceFiles GetSourceFiles(GithubDirectoryDescription description)
        {
            int limes = description.MainPath.LastIndexOf('/');
            string mainDirectory = description.MainPath.Substring(0, limes);
            string mainFile = description.MainPath.Substring(limes + 1);
            string apiUrl = $"https://api.github.com/repos/{description.Owner}/{description.Repository}/contents/{mainDirectory}";
            string response = GetHttp(apiUrl);
            var files = JsonConvert.DeserializeObject<List<ContentResponse>>(response, jsonSettings);
            var allSourceFiles = files
                .Select(f => new SourceFile
                {
                    File = f.Name,
                    Code = GetHttp(f.DownloadUrl)
                });
            return new SourceFiles
            {
                Code = allSourceFiles.Single(f => f.File == mainFile).Code,
                Codes = allSourceFiles.Where(f => f.File != mainFile).ToList()
            };
        }
    }
}
