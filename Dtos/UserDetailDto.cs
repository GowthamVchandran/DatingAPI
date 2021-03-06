using System;
using System.Collections.Generic;
using DatingAPI.Model;

namespace DatingAPI.Dtos
{
    public class UserDetailDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public int  Age { get; set; }
         public string KnownAs { get; set; }
         public DateTime Createed { get; set; }
         public DateTime LastActive { get; set; }    
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoURL { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interest { get; set; }
        public ICollection<PhotoDetailDto> Photos { get; set; }
    }
}