using System;
using System.Net.Http;

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

        private string GetHttp(string url)
        {
            return httpClient.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
        }

        public SourceFiles GetSourceFiles(GithubDirectoryDescription githubDirectoryDescription)
        {
            return new SourceFiles
            {
                Code = GetHttp(githubDirectoryDescription.Url)
            };
        }
    }
}
