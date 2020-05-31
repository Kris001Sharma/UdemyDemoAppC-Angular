using System.ComponentModel.DataAnnotations;

namespace DemoApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }       
        
        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "U must specify password between 4 - 8")]
        public string Password { get; set; }
    }
}