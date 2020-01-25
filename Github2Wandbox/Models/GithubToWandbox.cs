using System.Linq;
using System.Threading.Tasks;
using Github2Wandbox.Repository;

namespace Github2Wandbox.Models
{
    public class GithubToWandbox
    {
        GithubDirectoryCommitChecker githubDirectoryCommitChecker;
        IGithubScanner githubScanner;
        WandboxPublisher wandboxPublisher;
        PublicationsContext publicationsContext;

        public GithubToWandbox(
            PublicationsContext publicationsContext,
            GithubDirectoryCommitChecker githubDirectoryCommitChecker,
            IGithubScanner githubScanner,
            WandboxPublisher wandboxPublisher)
        {
            this.publicationsContext = publicationsContext;
            this.githubDirectoryCommitChecker = githubDirectoryCommitChecker;
            this.githubScanner = githubScanner;
            this.wandboxPublisher = wandboxPublisher;
        }

        public async Task<string> TransformAsync(TransformationDescription description)
        {
            var directoryDescription = description.GithubDirectoryDescription;
            var commitSha = await githubDirectoryCommitChecker.GetCommitShaAsync(directoryDescription);

            var lastPublications = publicationsContext.Publications
                .Where(p => (p.Owner == directoryDescription.Owner
                    && p.Repository == directoryDescription.Repository
                    && p.MainPath == directoryDescription.MainPath))
                .ToList();
            if (lastPublications.Count > 0)
            {
                var lastPublication = lastPublications.First();
                if (lastPublication.CommitSha == commitSha)
                    return lastPublication.Url;
            }

            var sourceFiles = await githubScanner.GetSourceFilesAsync(directoryDescription);
            var url = await wandboxPublisher.PublishAsync(sourceFiles, description.WandboxOptions);

            var publication = new Publication
            {
                Owner = directoryDescription.Owner,
                Repository = directoryDescription.Repository,
                MainPath = directoryDescription.MainPath,
                CommitSha = commitSha,
                Url = url
            };
            publicationsContext.Publications.Add(publication);
            await publicationsContext.SaveChangesAsync();

            return url;
        }
    }
}
