using System;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;
using Xunit.Abstractions;
using Github2Wandbox.Models.Wandbox.API;
using Github2Wandbox.Models.Common;

namespace WandboxApiExploration
{
    public class WandboxCompile
    {
        private readonly ITestOutputHelper output;

        public WandboxCompile(ITestOutputHelper output)
        {
            this.output = output;
        }

        string execute(string url, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var memoryStream = new MemoryStream(bytes);
            var streamContent = new StreamContent(memoryStream);
            streamContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
            return new HttpClient().PostAsync(url, streamContent).Result.Content.ReadAsStringAsync().Result;
        }

        [Fact]
        public void compiles_file()
        {
            var query = new CompileRequest
            {
                Compiler = "gcc-head",
                Code = "#include <iostream>\nint main(){std::cout << \"Hello from API\" << std::endl;}",
                Save = true
            };
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;
            var json = JsonConvert.SerializeObject(query, settings);
            string response = execute("https://wandbox.org/api/compile.json", json);
            var json_response = JsonConvert.DeserializeObject<CompileResponse>(response, settings);
            Assert.NotNull(json_response);
        }

        [Fact]
        public void compile_few_files()
        {
            var query = new CompileRequest
            {
                Compiler = "gcc-head",
                Code = "#include <iostream>\n#include \"header.h\"\nint main(){std::cout << f() << std::endl;}",
                Save = true,
                Codes = new System.Collections.Generic.List<SourceFile>
                {
                    new SourceFile
                    {
                        File="header.h",
                        Code="#include <string>\nstd::string f(){return \"Hello from API\";}"
                    },
                }
            };
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;
            var json = JsonConvert.SerializeObject(query, settings);
            string response = execute("https://wandbox.org/api/compile.json", json);
            var json_response = JsonConvert.DeserializeObject<CompileResponse>(response, settings);
            Assert.NotNull(json_response);
        }
    }
}