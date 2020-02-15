using System;

namespace Github2Wandbox.Models.Common
{
    public class SourceFile : IEquatable<SourceFile>
    {
        public string File { get; set; }
        public string Code { get; set; }

        public bool Equals(SourceFile other)
        {
            return other != null && (File == other.File) && (Code == other.Code);
        }
    }
}
