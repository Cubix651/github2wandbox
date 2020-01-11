using System.ComponentModel;

namespace Github2Wandbox.ViewModels
{
    public class OptionsViewModel
    {
        [DisplayName("Owner")]
        public string owner { get; set; }
        [DisplayName("Repository")]
        public string repository { get; set; }
        [DisplayName("Main path")]
        public string main_path { get; set; }
        [DisplayName("Compiler standard")]
        public string compiler_standard { get; set; }
    }
}
