using System;
using System.Collections.Generic;

namespace Github2Wandbox.Models.Common
{
    public class SourceFiles
    {
        public string Code { get; set; }
        public List<SourceFile> Codes { get; set; }
    }
}
