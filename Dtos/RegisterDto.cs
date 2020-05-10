using System;
using System.ComponentModel.DataAnnotations;

namespace DatingAPI.Dtos
{
    public class RegisterDto
    {
        public RegisterDto()
        {
            LastActive = DateTime.Now;
            Createed = DateTime.Now;
        }
        [Required]
        public string Username { get; set; }
        [Required]           
        public string Password { get; set; }
        [Required]    
        public string gender { get; set; }
        [Required]    
        public string knownAs { get; set; }
        public DateTime DOB { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public DateTime Createed { get; set; }
        public DateTime LastActive { get; set; }
    }
}