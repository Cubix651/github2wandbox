using System.Threading.Tasks;

namespace Github2Wandbox.Models
{
    public class GithubToWandbox
    {
        IGithubScanner githubScanner;
        WandboxPublisher wandboxPublisher;

        public GithubToWandbox(IGithubScanner githubScanner, WandboxPublisher wandboxPublisher)
        {
            this.githubScanner = githubScanner;
            this.wandboxPublisher = wandboxPublisher;
        }

        public async Task<string> TransformAsync(TransformationDescription description)
        {
            var sourceFiles = await githubScanner.GetSourceFilesAsync(description.GithubDirectoryDescription);
            return await wandboxPublisher.PublishAsync(sourceFiles, description.WandboxOptions);
        }
    }
}
