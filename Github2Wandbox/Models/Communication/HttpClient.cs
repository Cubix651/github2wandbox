using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Github2Wandbox.Models.Communication
{
    public class HttpClient : IHttpClient
    {
        System.Net.Http.HttpClient httpClient;

        public HttpClient()
        {
            httpClient = new System.Net.Http.HttpClient();
        }

        public async Task<string> GetAsync(string url)
        {
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public void AddUserAgent(string userAgent)
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
        }

        public async Task<string> PostAsync(string url, string content, string mimeType = null)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var memoryStream = new MemoryStream(bytes);
            var streamContent = new StreamContent(memoryStream);
            if (mimeType != null)
                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);
            var response = await httpClient.PostAsync(url, streamContent);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
