using System;
using System.Net.Http;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace WandboxApiExploration
{
    public class WandboxCompilerList
    {
        private readonly ITestOutputHelper output;

        public WandboxCompilerList(ITestOutputHelper output)
        {
            this.output = output;
        }

        string execute(string url)
        {
            return new HttpClient().GetAsync(url).Result.Content.ReadAsStringAsync().Result;
        }

        [Fact]
        public void gets_response()
        {
            string result = execute("https://wandbox.org/api/list.json");
            //output.WriteLine(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void gets_json_response()
        {
            string result = execute("https://wandbox.org/api/list.json");

            var json = JsonConvert.DeserializeObject(result);
            
            Assert.NotNull(json);
        }
    }
}
