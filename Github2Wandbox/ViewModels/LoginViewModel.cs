using System.ComponentModel.DataAnnotations;

namespace Github2Wandbox.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name="Remember?")]
        public bool Remember { get; set; }
    }
}
