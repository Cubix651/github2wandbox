using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Github2Wandbox.Models.Wandbox;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Github2Wandbox.Models
{
    public class WandboxPublisher
    {
        JsonSerializerSettings jsonSerializerSettings;
        HttpClient httpClient;

        public static string Url { get; } = "https://wandbox.org/api/compile.json";
        public static string Compiler { get; } = "gcc-head";

        public WandboxPublisher()
        {
            jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            httpClient = new HttpClient();
        }

        private string PostHttp(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var memoryStream = new MemoryStream(bytes);
            var streamContent = new StreamContent(memoryStream);
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            return httpClient.PostAsync(Url, streamContent).Result.Content.ReadAsStringAsync().Result;
        }

        private CompileResponse PostHttpViaJson(CompileRequest compileRequest)
        {
            var jsonRequest = JsonConvert.SerializeObject(compileRequest, jsonSerializerSettings);
            var jsonResponse = PostHttp(jsonRequest);
            return JsonConvert.DeserializeObject<CompileResponse>(jsonResponse);
        }

        public string Publish(SourceFiles sourceFiles, WandboxOptions options)
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
            var compileResponse = PostHttpViaJson(compileRequest);
            return compileResponse.Url;
        }
    }
}
