using System.Threading.Tasks;
using Github2Wandbox.Models.Common;
using Github2Wandbox.Models.Communication;

namespace Github2Wandbox.Models.Github
{
    public class GithubFileScanner : IGithubScanner
    {
        IHttpClient httpClient;

        public GithubFileScanner(IHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<SourceFiles> GetSourceFilesAsync(GithubDirectoryDescription githubDirectoryDescription)
        {
            return new SourceFiles
            {
                Code = await httpClient.GetAsync(githubDirectoryDescription.Url)
            };
        }
    }
}
