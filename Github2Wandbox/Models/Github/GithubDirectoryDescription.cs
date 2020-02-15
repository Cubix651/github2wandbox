using System;
using System.Diagnostics.CodeAnalysis;

namespace Github2Wandbox.Models.Github
{
    public class GithubDirectoryDescription : IEquatable<GithubDirectoryDescription>
    {
        public string Owner { get; set; }
        public string Repository { get; set; }
        public string MainPath { get; set; }

        public string Url
        {
            get => $"https://raw.githubusercontent.com/{Owner}/{Repository}/master/{MainPath}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GithubDirectoryDescription);
        }

        public bool Equals([AllowNull] GithubDirectoryDescription other)
        {
            return other != null &&
                   Owner == other.Owner &&
                   Repository == other.Repository &&
                   MainPath == other.MainPath;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Owner, Repository, MainPath);
        }
    }
}
