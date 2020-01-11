namespace Github2Wandbox.Models
{
    public interface IGithubScanner
    {
        SourceFiles GetSourceFiles(GithubDirectoryDescription description);
    }
}
