using System.Collections.Generic;
using Github2Wandbox.Models.Common;
using Newtonsoft.Json;

namespace Github2Wandbox.Models.Wandbox.API
{
    public class CompileRequest
    {
        public string Compiler { get; set; }
        public string Code { get; set; }
        public List<SourceFile> Codes { get; set; }
        public string Options { get; set; }
        [JsonProperty("compiler-option-raw")]
        public string CompilerOptionRaw { get; set; }
        public bool Save { get; set; }
    }
}
