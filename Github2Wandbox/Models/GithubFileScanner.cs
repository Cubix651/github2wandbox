using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Github2Wandbox.Models
{
    public class GithubFileScanner : IGithubScanner
    {
        HttpClient httpClient;

        public static string UserAgent { get; } = "Github2Wandbox";

        public GithubFileScanner()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        }

        private async Task<string> GetHttpAsync(string url)
        {
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<SourceFiles> GetSourceFilesAsync(GithubDirectoryDescription githubDirectoryDescription)
        {
            return new SourceFiles
            {
                Code = await GetHttpAsync(githubDirectoryDescription.Url)
            };
        }
    }
}
