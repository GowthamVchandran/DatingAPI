using System;
using Microsoft.AspNetCore.Http;

namespace DatingAPI.Dtos
{
    public class photoForCreatingDto
    {
        public string Url { get; set; }
        public IFormFile File {get;set;}
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }

        public string PublicId { get; set; }

        public photoForCreatingDto()
        {
            DateAdded=DateTime.Now;
        }
    }
}