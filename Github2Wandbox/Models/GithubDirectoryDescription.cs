namespace Github2Wandbox.Models
{
    public class GithubDirectoryDescription
    {
        public string Owner { get; set; }
        public string Repository { get; set; }
        public string MainPath { get; set; }

        public string Url
        {
            get => $"https://raw.githubusercontent.com/{Owner}/{Repository}/master/{MainPath}";
        }
    }
}
