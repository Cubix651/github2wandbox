using Github2Wandbox.Models.Github;
using Github2Wandbox.Models.Wandbox;

namespace Github2Wandbox.Models
{
    public class TransformationDescription
    {
        public GithubDirectoryDescription GithubDirectoryDescription { get; set; }
        public WandboxOptions WandboxOptions { get; set; }
    }
}
