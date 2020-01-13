using System.Threading.Tasks;

namespace Github2Wandbox.Models
{
    public interface IGithubScanner
    {
        Task<SourceFiles> GetSourceFilesAsync(GithubDirectoryDescription description);
    }
}
