using System.Collections.Generic;
using Newtonsoft.Json;

namespace Github2Wandbox.Models.Wandbox
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
