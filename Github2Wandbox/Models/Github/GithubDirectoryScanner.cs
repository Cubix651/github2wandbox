using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Github2Wandbox.Models.Common;
using Github2Wandbox.Models.Communication;
using Github2Wandbox.Models.Github.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Github2Wandbox.Models.Github
{
    public class GithubDirectoryScanner : IGithubScanner
    {
        IHttpClient httpClient;
        JsonSerializerSettings jsonSettings;

        public GithubDirectoryScanner(IHttpClient httpClient)
        {
            this.httpClient = httpClient;

            jsonSettings = new JsonSerializerSettings();
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public async Task<SourceFiles> GetSourceFilesAsync(GithubDirectoryDescription description)
        {
            string mainDirectory = Path.GetDirectoryName(description.MainPath);
            string mainFile = Path.GetFileName(description.MainPath);
            string apiUrl = $"https://api.github.com/repos/{description.Owner}/{description.Repository}/contents/{mainDirectory}";
            string response = await httpClient.GetAsync(apiUrl);
            var files = JsonConvert.DeserializeObject<List<ContentResponse>>(response, jsonSettings);
            var allSourceFiles = files
                .Select(f => new SourceFile
                {
                    File = f.Name,
                    Code = httpClient.GetAsync(f.DownloadUrl).Result
                });
            return new SourceFiles
            {
                Code = allSourceFiles.Single(f => f.File == mainFile).Code,
                Codes = allSourceFiles.Where(f => f.File != mainFile).ToList()
            };
        }
    }
}
