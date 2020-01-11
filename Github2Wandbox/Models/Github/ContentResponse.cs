using System;
using Newtonsoft.Json;

namespace Github2Wandbox.Models.Github
{
    public class ContentResponse
    {
        public string Type { get; set; }
        public string Name { get; set; }
        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; }
    }
}
