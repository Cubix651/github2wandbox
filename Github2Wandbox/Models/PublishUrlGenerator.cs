using System;
using System.Net;
using Github2Wandbox.ViewModels;

namespace Github2Wandbox.Models
{
    public class PublishUrlGenerator
    {
        public string Generate(OptionsViewModel options)
        {
            string compilerStandard = WebUtility.UrlEncode(options.compiler_standard);
            return $"Publish/{options.owner}/{options.repository}/{options.main_path}" +
                $"?{nameof(options.compiler_standard)}={compilerStandard}";
        }
    }
}
