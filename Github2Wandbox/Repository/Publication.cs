namespace Github2Wandbox.Repository
{
    public class Publication
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public string Repository { get; set; }
        public string MainPath { get; set; }
        public string CommitSha { get; set; }
        public string Url { get; set; }
    }
}
