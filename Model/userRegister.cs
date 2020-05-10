using System;
using System.ComponentModel.DataAnnotations;

namespace DatingAPI.Model
{
    public class userRegister
    {
      
        [Required]                
        public string username { get; set; }
        public string password { get; set; }
        public string gender { get; set; }
        public string knownAs { get; set; }
        public DateTime DOB { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
    }
}