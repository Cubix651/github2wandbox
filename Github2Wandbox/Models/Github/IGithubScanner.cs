using System.Threading.Tasks;
using Github2Wandbox.Models.Common;

namespace Github2Wandbox.Models.Github
{
    public interface IGithubScanner
    {
        Task<SourceFiles> GetSourceFilesAsync(GithubDirectoryDescription description);
    }
}
