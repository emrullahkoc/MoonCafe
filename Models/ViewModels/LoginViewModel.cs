using System.ComponentModel.DataAnnotations;

namespace MoonCafe.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string UserEmail { get; set; }

        [Required]
        public string UserPassword { get; set; }
    }
}
