using System;

namespace DatingAPI.Dtos
{
    public class UsersForDto
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
    }
}