using System;
using System.Linq;
using System.Threading.Tasks;
using Github2Wandbox.Models.Common;
using Github2Wandbox.Models.Communication;
using Github2Wandbox.Models.Wandbox.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Github2Wandbox.Models.Wandbox
{
    public class WandboxPublisher
    {
        JsonSerializerSettings jsonSerializerSettings;
        IHttpClient httpClient;

        public static string Url { get; } = "https://wandbox.org/api/compile.json";
        public static string Compiler { get; } = "gcc-head";

        public WandboxPublisher(IHttpClient httpClient)
        {
            jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            this.httpClient = httpClient;
        }

        private async Task<CompileResponse> PostHttpViaJsonAsync(CompileRequest compileRequest)
        {
            var jsonRequest = JsonConvert.SerializeObject(compileRequest, jsonSerializerSettings);
            var jsonResponse = await httpClient.PostAsync(Url, jsonRequest, "application/json");
            return JsonConvert.DeserializeObject<CompileResponse>(jsonResponse);
        }

        public virtual async Task<string> PublishAsync(SourceFiles sourceFiles, WandboxOptions options)
        {
            string sourcePaths = "";
            if (sourceFiles.Codes != null)
                sourcePaths = String.Join("\n", sourceFiles.Codes
                    .Where(c => c.File.EndsWith(".cpp"))
                    .Select(c => c.File));
            var compileRequest = new CompileRequest
            {
                Code = sourceFiles.Code,
                Codes = sourceFiles.Codes,
                Compiler = Compiler,
                Options = options.CompilerStandard,
                CompilerOptionRaw = sourcePaths,
                Save = true
            };
            var compileResponse = await PostHttpViaJsonAsync(compileRequest);
            return compileResponse.Url;
        }
    }
}
