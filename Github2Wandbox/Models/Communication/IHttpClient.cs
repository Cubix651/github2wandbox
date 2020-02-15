using System;
using System.Threading.Tasks;

namespace Github2Wandbox.Models.Communication
{
    public interface IHttpClient
    {
        Task<string> GetAsync(string url);
        void AddUserAgent(string userAgent);
        Task<string> PostAsync(string url, string content, string mimeType);
    }
}
