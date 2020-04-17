using System.ComponentModel.DataAnnotations;

namespace DatingAPI.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]        
        public string Password { get; set; }
    }
}