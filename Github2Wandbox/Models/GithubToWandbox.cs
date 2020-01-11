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

        public string Transform(TransformationDescription description)
        {
            var sourceFiles = githubScanner.GetSourceFiles(description.GithubDirectoryDescription);
            return wandboxPublisher.Publish(sourceFiles, description.WandboxOptions);
        }
    }
}
